using LifeQuest.BLL.DTOs;
using Microsoft.AspNetCore.Authorization;
using LifeQuest.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LifeQuest.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories == null)
            {
                ModelState.AddModelError("", "Categories not found");
                return View();
            }
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                ViewBag.ErrorMessage = "Category not found";
                return View();
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid Category");
                return View(dto);
            }
            
            var result = await _categoryService.CreateCategoryAsync(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to create category");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                ViewBag.ErrorMessage = "Category not found";
                return View();
            }

            return View(category);
        }

        // POST: Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid Category");
                return View(dto);
            }

            var result = await _categoryService.UpdateCategoryAsync(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to update category");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                ViewBag.ErrorMessage = "Category not found";
                return View();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to delete category");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}