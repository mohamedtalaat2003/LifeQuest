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

        private int GetUserId() => int.Parse(_userManager.GetUserId(User)!);

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

            try
            {
                var success = await _decisionService.AddDecisionAsync(dto);
                if (!success)
                {
                    ModelState.AddModelError("", "Failed to add decision");
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
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

            return View(decision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool isSuccess)
        {
            if (id <= 0) return BadRequest();

            try
            {
                var result = await _decisionService.UpdateDecisionResultAsync(id, isSuccess);
                if (!result)
                {
                    ModelState.AddModelError("", "Failed to update decision result");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Details), new { id });
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

            return View(decision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _decisionService.DeleteDecisionAsync(id);
                if (!result)
                {
                    ModelState.AddModelError("", "Failed to delete decision");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}