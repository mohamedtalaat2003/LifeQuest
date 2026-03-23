using LifeQuest.BLL.DTOs;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IApplicationUserLoginService
    {
        Task<SignInResult> LoginAsync(LoginDTO dto);
        Task LogoutAsync();
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByIdAsync(string userId);
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<string?> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<ExternalLoginInfo?> GetExternalLoginInfoAsync();
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor = false);
        Task<IdentityResult> AddLoginAsync(ApplicationUser user, ExternalLoginInfo info);
    }
}
