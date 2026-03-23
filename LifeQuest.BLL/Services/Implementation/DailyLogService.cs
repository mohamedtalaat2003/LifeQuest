using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using System.Numerics;

namespace LifeQuest.BLL.Services.Implementation
{
    public class DailyLogService : IDailyLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProfileService _userProfileService;

        public DailyLogService(IUnitOfWork unitOfWork, IMapper mapper, IUserProfileService userProfileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProfileService = userProfileService;
        }

        public async Task<bool> AddLogAsync(DailyLogDTO dailyLogDto, int userId)
        {
            // هبشوف المستخدم كان له تحدى ولا لا 
            var userChallenge = await _unitOfWork.Repository<UserChallenge>()
                .GetByIdWithIncludeAsync(uc => uc.ChallengeId == dailyLogDto.ChallengeId && uc.UserId == userId, "Challenge");

            if (userChallenge == null)
            {
                throw new Exception("The User Is Not Registered in this Challenge!");
            }

            // 2️⃣ منع التسجيل في المستقبل
            if (dailyLogDto.LogDate.Date > DateTime.Now.Date)
            {
                throw new Exception("Activity can not recorded at a future date!");
            }

            // منع التكرار في نفس اليوم 
            var existingLogs = await _unitOfWork.Repository<DailyLog>()
                .GetAllWithIncludesAsync(l => l.UserChallengeId == userChallenge.Id && l.LogDate.Date == dailyLogDto.LogDate.Date);
            
            if (existingLogs.Any())
            {
                throw new Exception("This day already recorded!");
            }

            // حساب النقاط حسب الصعوبه
            int difficultyPoints = userChallenge.Challenge?.Difficulty switch
            {
                "Easy" => 10,
                "Medium" => 20,
                "Hard" => 30,
                _ => 10
            };

            int pointsEarned = dailyLogDto.Duration * difficultyPoints;

            // هيكرت التسجيل اليومى
            var dailyLog = _mapper.Map<DailyLog>(dailyLogDto);
            dailyLog.Points = pointsEarned;
            dailyLog.UserChallengeId = userChallenge.Id;
            dailyLog.CurrentProgress = userChallenge.CurrentProgress + dailyLogDto.Duration;
            dailyLog.LogDate = dailyLogDto.LogDate;

            await _unitOfWork.Repository<DailyLog>().AddAsync(dailyLog);

            // هبحث التقدم تبع التحدى
            userChallenge.CurrentProgress += dailyLogDto.Duration;

            // تغيير حالة التحدى 
            if (userChallenge.Status == "NotStarted")
            {
                userChallenge.Status = "InProgress";
            }

            //  هيشوف المستخدم انجز التحدى ولا لسه 
            if (userChallenge.CurrentProgress >= userChallenge.Challenge?.Duration)
            {
                userChallenge.Status = "Ended";
                userChallenge.IsSuccess = true;
                
                // تحديث نسبة النجاح لليوزر بعد ما خلص التحدى بنجاح
                await _userProfileService.UpdateSuccessRateAsync(userId);
            }

            await _unitOfWork.Repository<UserChallenge>().Update(userChallenge);

            // 4️⃣ إضافة النقاط للمستخدم (وتحديث ليفله في نفس الوقت)
            await _userProfileService.AddPointsToUserAsync(userId, dailyLog.Points);

            // 5️⃣ تأكيد الحفظ لكل العمليات في تانزكشن واحدة
            var completeResult = await _unitOfWork.CompleteAsync();
            Console.WriteLine($"[DailyLogService] AddLogAsync for user {userId}: CompleteAsync result = {completeResult}");
            return completeResult > 0;
        }

        public async Task<double> GetChallengeProcessPrecentageAsync(int userChallengeId)
        {
            // هبشوف المستخدم كان له تحدى ولا لا 

            var userChallenge =await _unitOfWork.Repository<UserChallenge>()
                .GetByIdWithIncludeAsync(uc => uc.Id == userChallengeId, "Challenge");
            if (userChallenge == null)
            {
                throw new Exception("The User Is Not Registered in this Challenge!");
            }

            if (userChallenge.Challenge == null)
            {
                throw new Exception("Challenge details not found!");
            }

            // هنا هيحسب نسبة التقدم بناء على التقدم الى المستخدم واقف عنده من المده الكليه للتحدى
            double progress = ((double)userChallenge.CurrentProgress / userChallenge.Challenge.Duration) * 100;

            return Math.Round(progress,2);
        }

        public async Task<IEnumerable<DailyLogDTO>> GetLogsByDateAsync(int userId, DateTime date)
        {
            // هنا هجيب كل التسجيلات المرتطبطه بالمستخدم فى يوم او تاريخ معين 
            var Logs =await _unitOfWork.Repository<DailyLog>()
                .GetPagedAsync(1,5,X => X.UserChallenge != null && X.UserChallenge.UserId == userId && X.LogDate.Date == date.Date, "UserChallenge.Challenge");

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
