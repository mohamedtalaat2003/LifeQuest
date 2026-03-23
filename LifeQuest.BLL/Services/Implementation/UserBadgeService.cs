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
            // التحقق من استحقاق الاوسمه ومنحها للمستخدم
            // مثال: إذا أكمل المستخدم أول تحدي له، امنحه وسام "الخطوات الأولى"
            
            var completedChallenges = await _unitOfWork.Repository<UserChallenge>()
                .GetAllWithIncludesAsync(uc => uc.UserId == userId && uc.IsSuccess == true);

            if (completedChallenges.Any())
            {
                // التحقق مما إذا كان لديه بالفعل وسام "التحدي الأول" (نفترض المعرف 1 حاليًا)
                bool hasBadge = await _unitOfWork.Repository<UserBadge>()
                    .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == 1);

                if (!hasBadge)
                {
                    var userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = 1,
                        AwardedDate = DateTime.Now
                    };
                    await _unitOfWork.Repository<UserBadge>().AddAsync(userBadge);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }
    }
}
