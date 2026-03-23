using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LifeQuest.PL.Controllers
{
    [Authorize]
    public class UserBadgeController : Controller
    {
        private readonly IUserBadgeService _userBadgeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserBadgeController(IUserBadgeService userBadgeService, UserManager<ApplicationUser> userManager)
        {
            _userBadgeService = userBadgeService;
            _userManager = userManager;
        }

        private int GetUserId() => int.Parse(_userManager.GetUserId(User)!);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            var badges = await _userBadgeService.GetUserBadgesAsync(userId);

            if (badges == null || !badges.Any())
            {
                ViewBag.Message = "No badges awarded yet.";
                return View();
            }

            return View(badges);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAndAward()
        {
            int userId = GetUserId();
            await _userBadgeService.CheckAndAwardBadgesAsync(userId);
            return RedirectToAction(nameof(Index));
        }
    }
}