using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using Microsoft.Extensions.Logging;

namespace LifeQuest.BLL.Services.Implementation
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserBadgeService _userBadgeService;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper, IUserBadgeService userBadgeService, ILogger<UserProfileService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userBadgeService = userBadgeService;
            _logger = logger;
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int userId)
        {
            var userProfile = await _unitOfWork.Repository<UserProfile>()
                .GetByIdWithIncludeAsync(up => up.UserId == userId, "User", "Level", "User.UserBadges", "User.UserChallenges");

            if (userProfile == null)
            {
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
                
                userProfile = await _unitOfWork.Repository<UserProfile>()
                    .GetByIdWithIncludeAsync(up => up.UserId == userId, "User", "Level", "User.UserBadges", "User.UserChallenges");
            }

            var dto = _mapper.Map<UserProfileDTO>(userProfile);

            dto.TotalBadges = (await _unitOfWork.Repository<UserBadge>().GetAllWithIncludesAsync(ub => ub.UserId == userId)).Count();

            var userChallenges = await _unitOfWork.Repository<UserChallenge>().GetAllWithIncludesAsync(uc => uc.UserId == userId);
            dto.ActiveChallenges = userChallenges.Count(uc => uc.Status == ChallengeStatus.InProgress || uc.Status == ChallengeStatus.NotStarted);

            _logger.LogInformation("GetUserProfileAsync for user {UserId}: Active challenges found: {ActiveChallenges}, Total Points: {TotalPoints}", userId, dto.ActiveChallenges, userProfile!.TotalPoints);

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
                dto.RemainingPointsForNextLevel = 0;
            }

            return dto;
        }

        public async Task UpdateUserProfileAsync(UserProfileDTO dto)
        {
            var userProfile = await _unitOfWork.Repository<UserProfile>().GetByIdAsync(dto.UserId);
            if (userProfile != null)
            {
                userProfile.Bio = dto.Bio;
                
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
            var userProfile = await _unitOfWork.Repository<UserProfile>()
                .GetByIdWithIncludeAsync(up => up.UserId == userId);
            
            if (userProfile == null)
            {
                await GetUserProfileAsync(userId);
                userProfile = await _unitOfWork.Repository<UserProfile>().GetByIdWithIncludeAsync(up => up.UserId == userId);
            }

            if (userProfile != null)
            {
                userProfile.TotalPoints += points;
                await _unitOfWork.Repository<UserProfile>().Update(userProfile);
                await _unitOfWork.CompleteAsync();
                
                await CheckLevelUpAsync(userId);
                
                _logger.LogInformation("Points added to state for user {UserId}. Total: {TotalPoints}", userId, userProfile.TotalPoints);
            }
            else
            {
                _logger.LogError("Could not find/create profile for user {UserId}", userId);
            }
        }

        public async Task UpdateSuccessRateAsync(int userId)
        {
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
                    await _unitOfWork.CompleteAsync();
                    
                    await _userBadgeService.CheckAndAwardBadgesAsync(userId);
                    
                    _logger.LogInformation("User {UserId} leveled up to {LevelName}!", userId, nextLevel.LevelName);
                }
            }
        }
    }
}
