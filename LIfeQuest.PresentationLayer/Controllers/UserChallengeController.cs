using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LifeQuest.PL.Controllers
{
    [Authorize]
    public class UserChallengeController : Controller
    {
        private readonly IUserChallengeService _userChallengeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserChallengeController(IUserChallengeService userChallengeService, UserManager<ApplicationUser> userManager)
        {
            _userChallengeService = userChallengeService;
            _userManager = userManager;
        }

        private int GetUserId() => int.Parse(_userManager.GetUserId(User)!);

        [HttpGet]
        public async Task<IActionResult> UserChallenges()
        {
            int userId = GetUserId();
            var challenges = await _userChallengeService.GetUserChallengesAsync(userId);

            if (challenges == null || !challenges.Any())
                ViewBag.Message = "No challenges joined yet.";

            return View(challenges);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int challengeId)
        {
            if (challengeId <= 0)
                return BadRequest("Invalid challengeId.");

            int userId = GetUserId();

            try
            {
                var challenge = await _userChallengeService.GetChallengeDetailsAsync(userId, challengeId);
                return View(challenge);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(UserChallenges));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int challengeId)
        {
            if (challengeId <= 0)
                return BadRequest("Invalid challengeId.");

            int userId = GetUserId();

            try
            {
                await _userChallengeService.JoinChallengeAsync(userId, challengeId);
                TempData["SuccessMessage"] = "Successfully joined the challenge!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(UserChallenges));
        }
    }
}