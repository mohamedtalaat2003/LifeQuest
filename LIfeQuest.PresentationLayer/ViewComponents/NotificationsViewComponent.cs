using LifeQuest.BLL.DTOs;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LifeQuest.PresentationLayer.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsViewComponent(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdStr = _userManager.GetUserId(HttpContext.User);
            if (string.IsNullOrEmpty(userIdStr))
                return View(new List<NotificationDTO>());

            int userId = int.Parse(userIdStr);
            var notifications = new List<NotificationDTO>();

            // 1. Get recent logs (Points earned)
            var logs = await _unitOfWork.Repository<DailyLog>()
                .GetPagedAsync(1, 10, l => l.UserChallenge.UserId == userId, "UserChallenge.Challenge");

            foreach(var log in logs)
            {
                notifications.Add(new NotificationDTO
                {
                    Message = $"Earned {log.Points} pts from {log.UserChallenge.Challenge.Title}",
                    Date = log.LogDate,
                    Icon = "fa-star",
                    ColorClass = "text-warning bg-warning bg-opacity-10",
                    Link = $"/Challenges/Details/{log.UserChallenge.ChallengeId}"
                });
            }

            // 2. Get recent badges
            var badges = await _unitOfWork.Repository<UserBadge>()
                .GetPagedAsync(1, 5, b => b.UserId == userId, "Badge");

            foreach(var badge in badges)
            {
                notifications.Add(new NotificationDTO
                {
                    Message = $"Unlocked Badge: {badge.Badge.Name}",
                    Date = badge.AwardedDate,
                    Icon = badge.Badge.Image ?? "fa-medal",
                    ColorClass = "text-primary bg-primary bg-opacity-10",
                    Link = "/Reward/Index" // Or badge index
                });
            }

            // Combine and sort
            var orderedNotifications = notifications.OrderByDescending(n => n.Date).Take(5).ToList();

            return View(orderedNotifications);
        }
    }
}
