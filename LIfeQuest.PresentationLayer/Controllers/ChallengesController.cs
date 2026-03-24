using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.PresentationLayer.ViewModels.Challenges;
using LifeQuest.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace LifeQuest.PresentationLayer.Controllers
{
    [Authorize]
    public class ChallengesController : Controller
    {
        private readonly IChallengeService _challengeService;
        private readonly IUserChallengeService _userChallengeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChallengesController(IChallengeService challengeService, IUserChallengeService userChallengeService, UserManager<ApplicationUser> userManager)
        {
            _challengeService = challengeService;
            _userChallengeService = userChallengeService;
            _userManager = userManager;
        }

        // GET: Challenges
        public async Task<IActionResult> Index()
        {
            var userIdStr = _userManager.GetUserId(User);
            int? userId = string.IsNullOrEmpty(userIdStr) ? null : int.Parse(userIdStr);
            bool isAdmin = User.IsInRole("Admin");

            var challenges = await _challengeService.GetAllChallengesAsync(userId, isAdmin);
            var viewModel = challenges.Select(c => new ChallengeViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Points = c.Points,
                Duration = c.Duration,
                IsPublic = c.IsPublic,
                Difficulty = c.Difficulty,
                CreatedAt = c.CreatedAt
            }).ToList();

            return View(viewModel);
        }

        // GET: Challenges/Create
        public IActionResult Create(bool isPublic = false)
        {
            return View(new CreateChallengeViewModel { IsPublic = isPublic });
        }

        // POST: Challenges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateChallengeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool isPublic = viewModel.IsPublic;
                if (isPublic && !User.IsInRole("Admin"))
                {
                    isPublic = false; // Silently downgrade, or could add an error
                }

                var dto = new ChallengeDTO
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    Difficulty = viewModel.Difficulty,
                    IsPublic = isPublic,
                    ApplicationUserId = int.Parse(_userManager.GetUserId(User) ?? "0")
                };

                await _challengeService.CreateChallengeAsync(dto);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var challenge = await _challengeService.GetChallengeByIdAsync(id);
            if (challenge == null) return NotFound();

            var userIdStr = _userManager.GetUserId(User);
            bool isJoined = false;
            
            if (!string.IsNullOrEmpty(userIdStr))
            {
                var userChallenges = await _userChallengeService.GetUserChallengesAsync(int.Parse(userIdStr));
                isJoined = userChallenges.Any(uc => uc.ChallengeId == id);
            }

            ViewBag.IsJoined = isJoined;
            return View(challenge);
        }

        [HttpPost]
        [Authorize]
        [Route("Challenges/Join/{id}")]
        public async Task<IActionResult> Join(int id)
        {
            var userIdStr = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();

            await _userChallengeService.JoinChallengeAsync(int.Parse(userIdStr), id);
            return RedirectToAction(nameof(Details), new { id = id });
        }

        public IActionResult DailyCheckIn()
        {
            return View();
        }

        private int GetUserId()
        {
            var userIdStr = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdStr)) return 0;
            return int.Parse(userIdStr);
        }
    }
}
