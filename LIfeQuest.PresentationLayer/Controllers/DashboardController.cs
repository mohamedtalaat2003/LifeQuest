using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LifeQuest.PresentationLayer.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IUserProfileService userProfileService, UserManager<ApplicationUser> userManager)
        {
            _userProfileService = userProfileService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            
            var userExists = await _userManager.FindByIdAsync(userIdStr);
            if (userExists == null)
            {
                // Stale cookie from dropped database
                return RedirectToAction("Logout", "Account");
            }

            int userId = int.Parse(userIdStr);
            var profile = await _userProfileService.GetUserProfileAsync(userId);

            // Pass data to Layout for navbar
            ViewData["UserPoints"] = profile?.TotalPoints.ToString("N0") ?? "0";
            ViewData["UserLevel"] = profile?.LevelName ?? "Novice";
            
            return View(profile);
        }
    }
}
