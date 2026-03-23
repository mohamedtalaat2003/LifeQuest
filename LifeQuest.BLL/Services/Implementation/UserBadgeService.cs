using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Implementation
{
    public class UserBadgeService : IUserBadgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserBadgeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserBadgeDTO>> GetUserBadgesAsync(int userId)
        {
            // جلب كل الاوسمه الخاصه بالمستخدم
            var userBadges = await _unitOfWork.Repository<UserBadge>()
                .GetAllWithIncludesAsync(ub => ub.UserId == userId, "Badge");
            
            return _mapper.Map<IEnumerable<UserBadgeDTO>>(userBadges);
        }

        public async Task CheckAndAwardBadgesAsync(int userId)
        {
            // التحقق من استحقاق الاوسمه ومنحها للمستخدم بناءً على المعايير الجديدة
            
            // 1. هجيب كل الاوسمه المتاحه
            var allBadges = await _unitOfWork.Repository<Badges>().GetAllWithIncludesAsync(null, "RequiredLevel");
            
            // 2. هجيب الاوسمه اللى مع المستخدم حالياً عشان مكررهاش
            var userBadges = await _unitOfWork.Repository<UserBadge>().GetAllWithIncludesAsync(ub => ub.UserId == userId);
            var earnedBadgeIds = userBadges.Select(ub => ub.BadgeId).ToHashSet();

            // 3. هجيب شوية احصائيات لليوزر عشان نقارن بيها
            var userProfile = await _unitOfWork.Repository<UserProfile>().GetByIdWithIncludeAsync(up => up.UserId == userId, "Level");
            if (userProfile == null) return;

            var userChallenges = await _unitOfWork.Repository<UserChallenge>().GetAllWithIncludesAsync(uc => uc.UserId == userId && uc.IsSuccess == true);
            int completedCount = userChallenges.Count();

            // هجيب الـ Streak (بنجيبه من الـ DailyLogService)
            // ملحوظه: بما ان الـ UserBadgeService ميعرفش الـ DailyLogService (عشان الـ Circular Dependency)، 
            // هنحسبه هنا ببساطه او نعتمد على ان اللى بينادى هو اللى بيحدد. بس عشان اخليها self-contained:
            var logs = await _unitOfWork.Repository<DailyLog>().GetAllWithIncludesAsync(l => l.UserChallenge.UserId == userId);
            int currentStreak = 0;
            if (logs.Any())
            {
                var distinctDates = logs.Select(l => l.LogDate.Date).Distinct().OrderByDescending(d => d).ToList();
                DateTime expected = DateTime.Now.Date;
                if (distinctDates.First() >= expected.AddDays(-1))
                {
                    foreach (var date in distinctDates)
                    {
                        if (date == expected || date == expected.AddDays(-1)) // سماح بيوم لو لسه مخلصش النهاردة
                        {
                            currentStreak++;
                            expected = date.AddDays(-1);
                        }
                        else break;
                    }
                }
            }

            // 4. هلف على كل الاوسمه واشوف ايه اللى يستاهله ومخدوش
            foreach (var badge in allBadges)
            {
                if (earnedBadgeIds.Contains(badge.Id)) continue;

                bool shouldAward = badge.CriteriaType switch
                {
                    "Level" => (userProfile.Level.LevelsCount >= badge.CriteriaValue) || (badge.RequiredLevelId.HasValue && userProfile.LevelId == badge.RequiredLevelId),
                    "ChallengeCount" => completedCount >= badge.CriteriaValue,
                    "Streak" => currentStreak >= badge.CriteriaValue,
                    _ => false
                };

                // استثناء خاص ببادجات الليفل لو مربوطه بـ ID معين مباشرة
                if (badge.CriteriaType == "Level" && badge.RequiredLevelId.HasValue)
                {
                    // لو ليفل اليوزر الحالى هو نفس ليفل البادج او اعلى منه (بناء على عدد الليفلات)
                    var reqLevel = badge.RequiredLevel;
                    if (reqLevel != null && userProfile.Level.LevelsCount >= reqLevel.LevelsCount)
                    {
                        shouldAward = true;
                    }
                }

                if (shouldAward)
                {
                    var newUserBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        AwardedDate = DateTime.Now
                    };
                    await _unitOfWork.Repository<UserBadge>().AddAsync(newUserBadge);
                    earnedBadgeIds.Add(badge.Id); // عشان مكررش فى نفس اللفه لو فيه بادجات معتمده على بعض
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
