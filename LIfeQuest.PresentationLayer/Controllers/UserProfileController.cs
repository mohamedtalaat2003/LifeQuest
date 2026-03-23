using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LifeQuest.PL.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(IUserProfileService userProfileService, UserManager<ApplicationUser> userManager)
        {
            _userProfileService = userProfileService;
            _userManager = userManager;
        }

        private int GetUserId() => int.Parse(_userManager.GetUserId(User)!);

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            int userId = GetUserId();
            var profile = await _userProfileService.GetUserProfileAsync(userId);

            if (profile == null)
            {
                ViewBag.ErrorMessage = "User profile not found.";
                return View();
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserProfileDTO dto, IFormFile? ProfileImage, [FromServices] IWebHostEnvironment env)
        {
            if (!ModelState.IsValid)
                return View("Details", dto);

            dto.UserId = GetUserId();

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(env.WebRootPath, "images", "profiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(fileStream);
                }

                dto.ProfilePictureUrl = "/images/profiles/" + uniqueFileName;
            }

            await _userProfileService.UpdateUserProfileAsync(dto);

            return RedirectToAction(nameof(Details));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPoints(int points)
        {
            if (points <= 0)
                return BadRequest("Invalid input.");

            int userId = GetUserId();
            await _userProfileService.AddPointsToUserAsync(userId, points);

            return RedirectToAction(nameof(Details));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSuccessRate()
        {
            int userId = GetUserId();
            await _userProfileService.UpdateSuccessRateAsync(userId);

            return RedirectToAction(nameof(Details));
        }
    }
}