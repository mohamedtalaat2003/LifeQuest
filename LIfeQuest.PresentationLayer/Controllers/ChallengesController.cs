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
            var challenges = await _challengeService.GetAllChallengesAsync();
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
        public IActionResult Create()
        {
            return View(new CreateChallengeViewModel());
        }

        // POST: Challenges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateChallengeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var dto = new ChallengeDTO
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    Points = viewModel.Points,
                    IsPublic = viewModel.IsPublic,
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

        [Route("Challenges/Daily-CheckIn")]
        [Route("Challenges/DailyCheckIn")]
        public IActionResult DailyCheckIn()
        {
            return View();
        }
    }
}
