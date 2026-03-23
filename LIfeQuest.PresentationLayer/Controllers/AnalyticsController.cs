using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LifeQuest.PresentationLayer.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IMetricsCalcService _metricsService;
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDailyLogService _dailyLogService;

        public AnalyticsController(IMetricsCalcService metricsService, IUserProfileService userProfileService, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IDailyLogService dailyLogService)
        {
            _metricsService = metricsService;
            _userProfileService = userProfileService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _dailyLogService = dailyLogService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            var profile = await _userProfileService.GetUserProfileAsync(userId);
            
            // Get last 7 days points
            var logs = await _unitOfWork.Repository<DailyLog>()
                .GetAllWithIncludesAsync(l => l.UserChallenge.UserId == userId, "UserChallenge");
            
            var last7Days = Enumerable.Range(0, 7)
                .Select(offset => DateTime.Now.Date.AddDays(-offset))
                .OrderBy(d => d)
                .ToList();

            var pointsPerDay = last7Days.Select(date => 
                logs.Where(l => l.LogDate.Date == date).Sum(l => l.Points)
            ).ToList();

            var labels = last7Days.Select(d => d.ToString("MMM dd")).ToList();

            int streak = await _dailyLogService.GetUserSteakAsync(userId);

            ViewBag.ChartLabels = labels;
            ViewBag.ChartData = pointsPerDay;
            ViewBag.CurrentStreak = streak;

            return View(profile);
        }
    }
}
