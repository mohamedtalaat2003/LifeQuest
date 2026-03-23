using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeQuest.PL.Controllers
{
    public class BadgeController : Controller
    {
        private readonly IBadgeService _badgeService;
        private readonly IUserBadgeService _userBadgeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BadgeController(IBadgeService badgeService, IUserBadgeService userBadgeService, UserManager<ApplicationUser> userManager)
        {
            _badgeService = badgeService;
            _userBadgeService = userBadgeService;
            _userManager = userManager;
        }

        [HttpGet]
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

        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var badge = await _badgeService.GetBadgeByIdAsync(id);

            if (badge == null)
                return NotFound();

            return View(badge);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BadgeDTO badgeDTO)
        {
            if (!ModelState.IsValid)
                return View(badgeDTO);

            var exist = await _badgeService.GetBadgeByNameAsync(badgeDTO.Name);

            if (exist != null)
            {
                ModelState.AddModelError("Name", "Badge already exists");
                return View(badgeDTO);
            }

            await _badgeService.AddBadgeAsync(badgeDTO);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var badge = await _badgeService.GetBadgeByIdAsync(id);

            if (badge == null)
            {
                ModelState.AddModelError("", "Not found");
                return View();
            }

            return View(badge);
        }

        // POST: Badge/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BadgeDTO badgeDTO)
        {
            if (!ModelState.IsValid)
               {
                ModelState.AddModelError("", "Invalid Badge");
                return View(badgeDTO);
               }

            var badge = await _badgeService.GetBadgeByIdAsync(badgeDTO.Id);

            if (badge == null)
            {
                ModelState.AddModelError("", "Not found");
                return View(badgeDTO);
            }

            await _badgeService.UpdateBadgeAsync(badgeDTO);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var badge = await _badgeService.GetBadgeByIdAsync(id);

            if (badge == null)
            {
                ModelState.AddModelError("", "Not found");
                return View();
            }

            return View(badge);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
                return BadRequest();

            await _badgeService.DeleteBadgeAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}