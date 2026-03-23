using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LifeQuest.PresentationLayer.Controllers
{
    [Authorize]
    public class DailyLogController : Controller
    {
        private readonly IDailyLogService _dailyLogService;
        private readonly IUserChallengeService _userChallengeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DailyLogController(IDailyLogService dailyLogService, IUserChallengeService userChallengeService, UserManager<ApplicationUser> userManager)
        {
            _dailyLogService = dailyLogService;
            _userChallengeService = userChallengeService;
            _userManager = userManager;
        }

        private int GetUserId() => int.Parse(_userManager.GetUserId(User)!);

        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            // Show today's logs and streak
            var todayLogs = await _dailyLogService.GetLogsByDateAsync(userId, DateTime.Now);
            var streak = await _dailyLogService.GetUserSteakAsync(userId);
            var activeChallenges = await _userChallengeService.GetUserChallengesAsync(userId);

            ViewBag.Streak = streak;
            ViewBag.ActiveChallenges = activeChallenges.Where(uc => uc.Status == "InProgress" || uc.Status == "NotStarted").ToList();

            return View(todayLogs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int userId = GetUserId();

            var activeChallenges = await _userChallengeService.GetUserChallengesAsync(userId);
            Console.WriteLine($"[DailyLogController] Create (GET) for user {userId}: Active challenges found in service: {activeChallenges.Count()}");
            ViewBag.ActiveChallenges = activeChallenges.Where(uc => uc.Status == "InProgress" || uc.Status == "NotStarted").ToList();

            return View(new DailyLogDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailyLogDTO dto)
        {
            int userId = GetUserId();

            if (!ModelState.IsValid)
            {
                var activeChallenges = await _userChallengeService.GetUserChallengesAsync(userId);
                ViewBag.ActiveChallenges = activeChallenges.Where(uc => uc.Status == "InProgress" || uc.Status == "NotStarted").ToList();
                return View(dto);
            }

            try
            {
                var success = await _dailyLogService.AddLogAsync(dto, userId);
                if (!success)
                {
                    ModelState.AddModelError("", "Failed to add daily log.");
                    var activeChallenges = await _userChallengeService.GetUserChallengesAsync(userId);
                    ViewBag.ActiveChallenges = activeChallenges.Where(uc => uc.Status == "InProgress" || uc.Status == "NotStarted").ToList();
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var activeChallenges = await _userChallengeService.GetUserChallengesAsync(userId);
                ViewBag.ActiveChallenges = activeChallenges.Where(uc => uc.Status == "InProgress" || uc.Status == "NotStarted").ToList();
                return View(dto);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Progress(int userChallengeId)
        {
            if (userChallengeId <= 0) return BadRequest();

            try
            {
                var percentage = await _dailyLogService.GetChallengeProcessPrecentageAsync(userChallengeId);
                ViewBag.ProgressPercentage = percentage;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LogsByDate(DateTime? date)
        {
            int userId = GetUserId();
            var targetDate = date ?? DateTime.Now;

            var logs = await _dailyLogService.GetLogsByDateAsync(userId, targetDate);
            if (logs == null || !logs.Any())
            {
                ViewBag.ErrorMessage = "No logs found for this date";
                return View();
            }

            return View(logs);
        }

        [HttpGet]
        public async Task<IActionResult> Streak()
        {
            int userId = GetUserId();
            var streak = await _dailyLogService.GetUserSteakAsync(userId);
            ViewBag.Streak = streak;
            return View();
        }
    }
}