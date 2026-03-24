using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Exceptions;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using Microsoft.Extensions.Logging;

namespace LifeQuest.BLL.Services.Implementation
{
    public class DailyLogService : IDailyLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<DailyLogService> _logger;

        public DailyLogService(IUnitOfWork unitOfWork, IMapper mapper, IUserProfileService userProfileService, ILogger<DailyLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProfileService = userProfileService;
            _logger = logger;
        }

        public async Task<bool> AddLogAsync(DailyLogDTO dailyLogDto, int userId)
        {
            var userChallenge = await _unitOfWork.Repository<UserChallenge>()
                .GetByIdWithIncludeAsync(uc => uc.ChallengeId == dailyLogDto.ChallengeId && uc.UserId == userId, "Challenge");

            if (userChallenge == null)
            {
                throw new NotFoundException("UserChallenge", $"UserId={userId}, ChallengeId={dailyLogDto.ChallengeId}");
            }

            if (dailyLogDto.LogDate.Date > DateTime.Now.Date)
            {
                throw new BusinessRuleException("Activity cannot be recorded at a future date!");
            }

            var existingLogs = await _unitOfWork.Repository<DailyLog>()
                .GetAllWithIncludesAsync(l => l.UserChallengeId == userChallenge.Id && l.LogDate.Date == dailyLogDto.LogDate.Date);
            
            if (existingLogs.Any())
            {
                throw new BusinessRuleException("This day has already been recorded!");
            }

            int difficultyPoints = userChallenge.Challenge?.Difficulty switch
            {
                ChallengeDifficulty.Easy => 10,
                ChallengeDifficulty.Medium => 20,
                ChallengeDifficulty.Hard => 30,
                _ => 10
            };

            int pointsEarned = dailyLogDto.Duration * difficultyPoints;

            var dailyLog = _mapper.Map<DailyLog>(dailyLogDto);
            dailyLog.Points = pointsEarned;
            dailyLog.UserChallengeId = userChallenge.Id;
            dailyLog.CurrentProgress = userChallenge.CurrentProgress + dailyLogDto.Duration;
            dailyLog.LogDate = dailyLogDto.LogDate;

            await _unitOfWork.Repository<DailyLog>().AddAsync(dailyLog);

            userChallenge.CurrentProgress += dailyLogDto.Duration;

            if (userChallenge.Status == ChallengeStatus.NotStarted)
            {
                userChallenge.Status = ChallengeStatus.InProgress;
            }

            if (userChallenge.CurrentProgress >= userChallenge.Challenge?.Duration)
            {
                userChallenge.Status = ChallengeStatus.Ended;
                userChallenge.IsSuccess = true;
                
                await _userProfileService.UpdateSuccessRateAsync(userId);
            }

            await _unitOfWork.Repository<UserChallenge>().Update(userChallenge);

            await _userProfileService.AddPointsToUserAsync(userId, dailyLog.Points);

            var completeResult = await _unitOfWork.CompleteAsync();
            _logger.LogInformation("AddLogAsync for user {UserId}: CompleteAsync result = {Result}", userId, completeResult);
            return completeResult > 0;
        }

        public async Task<double> GetChallengeProcessPrecentageAsync(int userChallengeId)
        {
            var userChallenge = await _unitOfWork.Repository<UserChallenge>()
                .GetByIdWithIncludeAsync(uc => uc.Id == userChallengeId, "Challenge");
            if (userChallenge == null)
            {
                throw new NotFoundException("UserChallenge", userChallengeId);
            }

            if (userChallenge.Challenge == null)
            {
                throw new NotFoundException("Challenge", "details not found for UserChallenge " + userChallengeId);
            }

            double progress = ((double)userChallenge.CurrentProgress / userChallenge.Challenge.Duration) * 100;

            return Math.Round(progress, 2);
        }

        public async Task<IEnumerable<DailyLogDTO>> GetLogsByDateAsync(int userId, DateTime date)
        {
            var Logs = await _unitOfWork.Repository<DailyLog>()
                .GetPagedAsync(1, 5, X => X.UserChallenge != null && X.UserChallenge.UserId == userId && X.LogDate.Date == date.Date, "UserChallenge.Challenge");

            if (Logs == null) return Enumerable.Empty<DailyLogDTO>();

            var logsDTO = _mapper.Map<IEnumerable<DailyLogDTO>>(Logs);

            return logsDTO;
        }

        public async Task<int> GetUserSteakAsync(int userId)
        {
            var logs = await _unitOfWork.Repository<DailyLog>()
                .GetAllWithIncludesAsync(l => l.UserChallenge != null && l.UserChallenge.UserId == userId, "UserChallenge");

            if (logs == null || !logs.Any()) return 0;

            var distinctDates = logs
                .Select(l => l.LogDate.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            int streak = 0;
            DateTime today = DateTime.Now.Date;

            if (distinctDates[0] < today.AddDays(-1))
            {
                return 0;
            }

            DateTime expectedDate = distinctDates[0];

            foreach (var logDate in distinctDates)
            {
                if (logDate == expectedDate)
                {
                    streak++;
                    expectedDate = expectedDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

    }
}
