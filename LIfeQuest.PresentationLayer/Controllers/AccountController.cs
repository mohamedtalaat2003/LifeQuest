using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace LifeQuest.PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApplicationUserRegisterService _registerService;
        private readonly IApplicationUserLoginService _loginService;
        private readonly IEmailSenderService _emailSenderService;

        public AccountController(
            IApplicationUserRegisterService registerService,
            IApplicationUserLoginService loginService,
            IEmailSenderService emailSenderService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _emailSenderService = emailSenderService;
        }

        // ================= REGISTER =================

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var vm = new RegistrationVM
            {
                Countries = await _registerService.Countries()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Countries = await _registerService.Countries();
                return View(vm);
            }

            if (vm.PlainPassword != vm.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                vm.Countries = await _registerService.Countries();
                return View(vm);
            }

            // Map VM → DTO
            var dto = new RegisterationDTO
            {
                Name = vm.Name,
                UserName = vm.UserName,
                Email = vm.Email,
                PlainPassword = vm.PlainPassword,
                Country = vm.Country
            };

            var (user, errors) = await _registerService.CreateAsync(dto, vm.PlainPassword);

            if (errors.Any())
            {
                foreach (var error in errors)
                    ModelState.AddModelError("", error);

                vm.Countries = await _registerService.Countries();
                return View(vm);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Error creating user");
                vm.Countries = await _registerService.Countries();
                return View(vm);
            }

            // Generate email confirmation token via BLL
            var token = await _loginService.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token },
                Request.Scheme);

            Console.WriteLine($"[DEBUG] Confirmation Link: {link}");

            try
            {
                await _emailSenderService.SendEmailAsync(
                    user.Email!,
                    "Confirm your email",
                    $"Click <a href='{link}'>here</a> to confirm your email");
            }
            catch
            {
                // Email sending failed, but user was created successfully
            }

            return RedirectToAction("CheckYourEmail");
        }

        public IActionResult CheckYourEmail()
        {
            return View();
        }

        // ================= CONFIRM EMAIL =================

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest();

            var success = await _loginService.ConfirmEmailAsync(userId, token);

            if (success)
                return View("EmailConfirmed");

            return BadRequest("Error confirming email");
        }

        public IActionResult EmailConfirmed()
        {
            return View();
        }

        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required");

            var user = await _loginService.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            if (user.EmailConfirmed)
                return BadRequest("Email already confirmed");

            var token = await _loginService.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token },
                Request.Scheme);

            await _emailSenderService.SendEmailAsync(
                user.Email!,
                "Confirm your email",
                $"Click <a href='{link}'>here</a>");

            return Ok("Email sent");
        }

        // ================= LOGIN =================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Map VM → DTO
            var dto = new LoginDTO
            {
                UserName = vm.UserName,
                Password = vm.PlainPassword,
                RememberMe = vm.RememberMe
            };

            var result = await _loginService.LoginAsync(dto);

            if (result == Microsoft.AspNetCore.Identity.SignInResult.NotAllowed)
            {
                ModelState.AddModelError("", "Please confirm your email first");
                return View(vm);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(vm);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _loginService.ConfigureExternalAuthenticationProperties(provider, redirectUrl ?? "/");
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login");
            }

            var info = await _loginService.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _loginService.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationVM { Email = email ?? "" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationVM vm, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var info = await _loginService.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction("Login");
                }

                // Create a basic DTO for registration
                var dto = new RegisterationDTO
                {
                    Email = vm.Email,
                    UserName = vm.Email,
                    Name = vm.Email,
                    Country = "Default" // Or handle appropriately
                };

                var (user, errors) = await _registerService.CreateAsync(dto, "ExternalLogin123!"); // Random password for external login

                if (user != null)
                {
                    // Add the external login to the user
                    var addLoginResult = await _loginService.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        var loginResult = await _loginService.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                        if (loginResult.Succeeded)
                        {
                            return LocalRedirect(returnUrl);
                        }
                    }
                }

                foreach (var error in errors)
                    ModelState.AddModelError("", error);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(vm);
        }

        // ================= LOGOUT =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _loginService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ================= FORGOT PASSWORD =================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var token = await _loginService.GeneratePasswordResetTokenAsync(vm.Email);

            // Always redirect to confirmation (don't reveal if user exists)
            if (token == null)
                return RedirectToAction("ForgotPasswordConfirmation");

            var link = Url.Action("ResetPassword", "Account",
                new { email = vm.Email, token = token },
                Request.Scheme);

            await _emailSenderService.SendEmailAsync(
                vm.Email,
                "Reset Password",
                $"Click <a href='{link}'>here</a> to reset your password");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // ================= RESET PASSWORD =================

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordVM
            {
                Email = email,
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _loginService.ResetPasswordAsync(vm.Email, vm.Token, vm.PlainPassword);

            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}