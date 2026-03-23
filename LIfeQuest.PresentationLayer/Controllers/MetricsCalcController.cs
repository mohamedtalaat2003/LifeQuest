using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LifeQuest.PL.Controllers
{
    public class MetricsCalcController : Controller
    {
        private readonly IMetricsCalcService _metricsService;

        public MetricsCalcController(IMetricsCalcService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet]
        public async Task<IActionResult> UserMetrics(int userId, int decisionId)
        {
            if (userId <= 0 || decisionId <= 0)
            {
                return BadRequest("Invalid userId or decisionId.");
            }

            var metrics = await _metricsService.CalculateUserMetricsAsync(userId, decisionId);

            // لو مفيش بيانات، نعرض رسالة
            if (metrics == null)
            {
                ViewBag.ErrorMessage = "No metrics found for this user/decision.";
                return View();
            }

            return View(metrics);
        }
    }
}