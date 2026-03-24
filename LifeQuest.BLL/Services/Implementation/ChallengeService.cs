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
    public class ChallengeService : IChallengeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChallengeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateChallengeAsync(ChallengeDTO dto)
        {
            var challenge = _mapper.Map<Challenge>(dto);
            
            // Automatic Point Calculation based on Difficulty
            challenge.Points = dto.Difficulty switch
            {
                ChallengeDifficulty.Easy => 50,
                ChallengeDifficulty.Medium => 100,
                ChallengeDifficulty.Hard => 200,
                _ => 100
            };

            challenge.CategoryId = dto.CategoryId == 0 ? 1 : dto.CategoryId; 
            challenge.Duration = (dto.EndDate - dto.StartDate).Days;
            
            await _unitOfWork.Repository<Challenge>().AddAsync(challenge);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<ChallengeDTO>> GetAllChallengesAsync(int? userId = null, bool isAdmin = false)
        {
            // If Admin, get all. Otherwise, get public + owned.
            var challenges = await _unitOfWork.Repository<Challenge>()
                .GetAllWithIncludesAsync(c => isAdmin || c.IsPublic || (userId.HasValue && c.ApplicationUserId == userId), "Category");
            
            return _mapper.Map<List<ChallengeDTO>>(challenges.ToList());
        }

        public async Task<ChallengeDTO?> GetChallengeByIdAsync(int id)
        {
            // هجيب تحدى واحد بس بال ID بتاعه وبالمرة ال Category بتاعته
            var challenge = await _unitOfWork.Repository<Challenge>().GetByIdWithIncludeAsync(c => c.Id == id, "Category");
            return _mapper.Map<ChallengeDTO>(challenge);
        }

        public async Task<bool> UpdateChallengeAsync(ChallengeDTO dto)
        {
            // هعدل بيانات التحدى اللى جايلى
            var challenge = _mapper.Map<Challenge>(dto);
            challenge.Duration = (dto.EndDate - dto.StartDate).Days;
            await _unitOfWork.Repository<Challenge>().Update(challenge);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteChallengeAsync(int id)
        {
            // هسمح للمستخدم يمسح التحدى خالص بال ID بتاعه
            await _unitOfWork.Repository<Challenge>().Delete(id);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ChallengeDTO>> GetChallengesByCategoryAsync(int categoryId, int? userId = null)
        {
            // هجيب كل التحديات اللى تبع Category معينة (العامة منها أو الخاصة بالمستخدم)
            var challenges = await _unitOfWork.Repository<Challenge>()
                .GetAllWithIncludesAsync(c => c.CategoryId == categoryId && (c.IsPublic || (userId.HasValue && c.ApplicationUserId == userId)), "Category");
            
            return _mapper.Map<IEnumerable<ChallengeDTO>>(challenges);
        }
    }
}
