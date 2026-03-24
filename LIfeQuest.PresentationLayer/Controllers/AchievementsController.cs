using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LifeQuest.PL.Controllers
{
    [Authorize]
    public class AchievementsController : Controller
    {
        private readonly IBadgeService _badgeService;
        private readonly IUserBadgeService _userBadgeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AchievementsController(IBadgeService badgeService, IUserBadgeService userBadgeService, UserManager<ApplicationUser> userManager)
        {
            _badgeService = badgeService;
            _userBadgeService = userBadgeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var allBadges = await _badgeService.GetAllBadgesAsync();
            var userIdStr = _userManager.GetUserId(User);
            
            if (!string.IsNullOrEmpty(userIdStr))
            {
                var userBadges = await _userBadgeService.GetUserBadgesAsync(int.Parse(userIdStr));
                ViewBag.UserBadgeIds = userBadges?.Select(ub => ub.BadgeId).ToList() ?? new List<int>();
            }
            else
            {
                ViewBag.UserBadgeIds = new List<int>();
            }

            return View(allBadges);
        }
    }
}
