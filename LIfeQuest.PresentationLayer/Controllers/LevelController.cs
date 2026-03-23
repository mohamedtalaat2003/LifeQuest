using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LifeQuest.PL.Controllers
{
    public class LevelController : Controller
    {
        private readonly ILevelService _levelService;

        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var levels = await _levelService.GetAllLevelsAsync();
            return View(levels);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var level = await _levelService.GetLevelByIdAsync(id);
            if (level == null)
            {
                ViewBag.ErrorMessage = "Level not found";
                return View();
            }

            return View(level);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LevelDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var success = await _levelService.AddLevelAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to add level");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest();

            var level = await _levelService.GetLevelByIdAsync(id);
            if (level == null)
            {
                ViewBag.ErrorMessage = "Level not found";
                return View();
            }

            return View(level);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LevelDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var success = await _levelService.UpdateLevelAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to update level");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var level = await _levelService.GetLevelByIdAsync(id);
            if (level == null)
            {
                ViewBag.ErrorMessage = "Level not found";
                return View();
            }

            return View(level);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _levelService.DeleteLevelAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to delete level");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}