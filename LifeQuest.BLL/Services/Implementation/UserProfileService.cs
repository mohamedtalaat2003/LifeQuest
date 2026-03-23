using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;

namespace LifeQuest.BLL.Services.Implementation
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserBadgeService _userBadgeService;

        public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper, IUserBadgeService userBadgeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userBadgeService = userBadgeService;
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int userId)
        {
            // هجيب بيانات الملف الشخصي بتاع اليوزر بكل الحاجات اللي معاه (badges, challenges, level)
            var userProfile = await _unitOfWork.Repository<UserProfile>()
                .GetByIdWithIncludeAsync(up => up.UserId == userId, "User", "Level", "User.UserBadges", "User.UserChallenges");

            if (userProfile == null)
            {
                // لو مش موجود، هنكريت واحد جديد بالمرة
                var defaultLevel = await _unitOfWork.Repository<Level>().GetAllAsync();
                var firstLevel = defaultLevel.OrderBy(l => l.Point).FirstOrDefault();
                
                userProfile = new UserProfile
                {
                    UserId = userId,
                    LevelId = firstLevel?.Id ?? 1,
                    TotalPoints = 0,
                    SuccessRate = 0,
                    Bio = "No bio yet."
                };
                await _unitOfWork.Repository<UserProfile>().AddAsync(userProfile);
                await _unitOfWork.CompleteAsync();
                
                // هنجيبه تانى عشان يبقى معاه ال includes
                userProfile = await _unitOfWork.Repository<UserProfile>()
                    .GetByIdWithIncludeAsync(up => up.UserId == userId, "User", "Level", "User.UserBadges", "User.UserChallenges");
            }

            var dto = _mapper.Map<UserProfileDTO>(userProfile);

            // هنحسب عدد البادجات والتحديات اللي لسه شغالة يدوي من الريبوزيتوري مباشرة عشان نضمن الدقة
            dto.TotalBadges = (await _unitOfWork.Repository<UserBadge>().GetAllWithIncludesAsync(ub => ub.UserId == userId)).Count();

            var userChallenges = await _unitOfWork.Repository<UserChallenge>().GetAllWithIncludesAsync(uc => uc.UserId == userId);
            dto.ActiveChallenges = userChallenges.Count(uc => uc.Status == "InProgress" || uc.Status == "NotStarted");

            Console.WriteLine($"[UserProfileService] GetUserProfileAsync for user {userId}: Active challenges found: {dto.ActiveChallenges}, Total Points: {userProfile!.TotalPoints}");

            // هحسب النقاط اللي ناقصة عشان يطلع للمستوى اللي بعده
            var allLevels = await _unitOfWork.Repository<Level>().GetAllAsync();
            var nextLevel = allLevels
                .Where(l => l.Point > userProfile.TotalPoints)
                .OrderBy(l => l.Point)
                .FirstOrDefault();

            if (nextLevel != null)
            {
                dto.RemainingPointsForNextLevel = nextLevel.Point - userProfile.TotalPoints;
            }
            else
            {
                dto.RemainingPointsForNextLevel = 0; // وصل لأعلى مستوى خلاص
            }

            return dto;
        }

        public async Task UpdateUserProfileAsync(UserProfileDTO dto)
        {
            // هعدل بيانات الملف الشخصي (زي الـ Bio)
            var userProfile = await _unitOfWork.Repository<UserProfile>().GetByIdAsync(dto.UserId);
            if (userProfile != null)
            {
                userProfile.Bio = dto.Bio;
                
                // Only update the profile picture URL if a new one was provided
                if (!string.IsNullOrEmpty(dto.ProfilePictureUrl))
                {
                    userProfile.ProfilePictureUrl = dto.ProfilePictureUrl;
                }

                await _unitOfWork.Repository<UserProfile>().Update(userProfile);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task AddPointsToUserAsync(int userId, int points)
        {
            // هزود نقاط لليوزر وهشوف لو يستاهل يترقى لمستوى أعلى
            var userProfile = await _unitOfWork.Repository<UserProfile>()
                .GetByIdWithIncludeAsync(up => up.UserId == userId); // استعملنا بريديكت عشان نضمن الدقة
            
            if (userProfile == null)
            {
                // لو مش موجود نكرته (ممكن يكون لسبب ما متكرتش وقت التسجيل)
                await GetUserProfileAsync(userId);
                userProfile = await _unitOfWork.Repository<UserProfile>().GetByIdWithIncludeAsync(up => up.UserId == userId);
            }

            if (userProfile != null)
            {
                userProfile.TotalPoints += points;
                await _unitOfWork.Repository<UserProfile>().Update(userProfile);
                
                // هعمل تشيك على الليفل بعد ما زودنا النقاط (دي بـ تأثر على الـ state بس)
                await CheckLevelUpAsync(userId);
                
                // await _unitOfWork.CompleteAsync(); // شيلنا دي عشان اللي بينادي هو اللي يقرر يحفظ امتى
                Console.WriteLine($"[UserProfileService] Points added to state for user {userId}. Total: {userProfile.TotalPoints}");
            }
            else
            {
                Console.WriteLine($"[UserProfileService] ERROR: Could not find/create profile for user {userId}");
            }
        }

        public async Task UpdateSuccessRateAsync(int userId)
        {
            // هحسب نسبة النجاح بتاعته بناءً على التحديات اللي خلصها صح
            var userChallenges = await _unitOfWork.Repository<UserChallenge>()
                .GetAllWithIncludesAsync(uc => uc.UserId == userId);

            int totalChallenges = userChallenges.Count();
            if (totalChallenges > 0)
            {
                int successfulChallenges = userChallenges.Count(uc => uc.IsSuccess);
                var userProfile = await _unitOfWork.Repository<UserProfile>().GetByIdAsync(userId);
                if (userProfile != null)
                {
                    userProfile.SuccessRate = (int)((double)successfulChallenges / totalChallenges * 100);
                    await _unitOfWork.Repository<UserProfile>().Update(userProfile);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }

        public async Task CheckLevelUpAsync(int userId)
        {
            // هشوف إجمالي نقاط اليوزر وهرقيه للمستوى المناسب لنقاطه
            var userProfile = await _unitOfWork.Repository<UserProfile>().GetByIdWithIncludeAsync(up => up.UserId == userId);
            if (userProfile != null)
            {
                var levels = await _unitOfWork.Repository<Level>().GetAllAsync();
                var nextLevel = levels
                    .Where(l => l.Point <= userProfile.TotalPoints)
                    .OrderByDescending(l => l.Point)
                    .FirstOrDefault();

                if (nextLevel != null && nextLevel.Id != userProfile.LevelId)
                {
                    userProfile.LevelId = nextLevel.Id;
                    await _unitOfWork.Repository<UserProfile>().Update(userProfile);
                    
                    // التحقق من منح أوسمة جديدة بعد الترقي
                    await _userBadgeService.CheckAndAwardBadgesAsync(userId);
                    
                    Console.WriteLine($"[UserProfileService] User {userId} leveled up to {nextLevel.LevelName}!");
                }
            }
        }
    }
}
