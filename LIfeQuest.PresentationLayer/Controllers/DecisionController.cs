using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LifeQuest.PL.Controllers
{
    [Authorize]
    public class DecisionController : Controller
    {
        private readonly IDecisionService _decisionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DecisionController(IDecisionService decisionService, UserManager<ApplicationUser> userManager)
        {
            _decisionService = decisionService;
            _userManager = userManager;
        }

        private int GetUserId()
        {
            var id = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return int.Parse(id);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            var decisions = await _decisionService.GetUserDecisionsAsync(userId);
            return View(decisions);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var decision = await _decisionService.GetDecisionDetailsAsync(id);
            if (decision == null)
            {
                TempData["ErrorMessage"] = "Decision not found";
                return RedirectToAction(nameof(Index));
            }

            // Authorization: verify ownership
            if (decision.UserId != GetUserId())
                return Forbid();

            return View(decision);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DecisionDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            dto.UserId = GetUserId();

            var success = await _decisionService.AddDecisionAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to add decision");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest();

            var decision = await _decisionService.GetDecisionDetailsAsync(id);
            if (decision == null)
            {
                TempData["ErrorMessage"] = "Decision not found";
                return RedirectToAction(nameof(Index));
            }

            // Authorization: verify ownership
            if (decision.UserId != GetUserId())
                return Forbid();

            return View(decision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResult(int id, bool isSuccess)
        {
            if (id <= 0) return BadRequest();

            // Authorization: verify ownership
            var decision = await _decisionService.GetDecisionDetailsAsync(id);
            if (decision != null && decision.UserId != GetUserId())
                return Forbid();

            var result = await _decisionService.UpdateDecisionResultAsync(id, isSuccess);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to update decision result";
            }
            else
            {
                TempData["SuccessMessage"] = $"Decision marked as {(isSuccess ? "Success" : "Failed")}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var decision = await _decisionService.GetDecisionDetailsAsync(id);
            if (decision == null)
            {
                TempData["ErrorMessage"] = "Decision not found";
                return RedirectToAction(nameof(Index));
            }

            // Authorization: verify ownership
            if (decision.UserId != GetUserId())
                return Forbid();

            return View(decision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Authorization: verify ownership
            var decision = await _decisionService.GetDecisionDetailsAsync(id);
            if (decision != null && decision.UserId != GetUserId())
                return Forbid();

            var result = await _decisionService.DeleteDecisionAsync(id);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to delete decision");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}