using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;

namespace LifeQuest.BLL.Services.Implementation
{
    public class UserChallengeService : IUserChallengeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserChallengeService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<UserChallengeDTO> GetChallengeDetailsAsync(int userId , int challengeId)
        {
            var userChallenge =await _unitOfWork.Repository<UserChallenge>()
                .GetByIdWithIncludeAsync(X => X.UserId == userId && X.ChallengeId == challengeId , "Challenge");

            if(userChallenge == null)
            {
                throw new Exception("User not registered in this challenge");
            }

            var userChallengeDTO = _mapper.Map<UserChallengeDTO>(userChallenge);

            return userChallengeDTO;
        }

        public async Task<IEnumerable<UserChallengeDTO>> GetUserChallengesAsync(int userId)
        {
            var challengesOfUser =await _unitOfWork.Repository<UserChallenge>()
                .GetAllWithIncludesAsync(X => X.UserId == userId, "Challenge");

            var challengesOfUserDTO = _mapper.Map<IEnumerable<UserChallengeDTO>>(challengesOfUser);

            return challengesOfUserDTO;
        }

        public async Task JoinChallengeAsync(int userId, int challengeId)
        {
            var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(userId);

            if (user == null) throw new KeyNotFoundException("User accounts was not found.");

            var challenge = await _unitOfWork.Repository<Challenge>().GetByIdAsync(challengeId);

            if (challenge == null) throw new KeyNotFoundException("The selected challenge no longer exists.");

            var isChallengeExist = await _unitOfWork.Repository<UserChallenge>()
              .AnyAsync(X => X.UserId == userId && X.ChallengeId == challengeId);

            if (isChallengeExist)
                throw new InvalidOperationException("You are already joined this challenge.");

            var userChallenge = new UserChallenge
            {
                ChallengeId = challengeId,
                UserId = userId,
                StartDate = DateTime.Now,
                Status = "NotStarted",
                CurrentProgress = 0,
                IsSuccess = false,
            };

            await _unitOfWork.Repository<UserChallenge>().AddAsync(userChallenge);

            int result = await _unitOfWork.CompleteAsync();

            if(result <= 0)
            {
                throw new Exception("An error occurred while joining the challenge.");
            }
        }
    }
}
