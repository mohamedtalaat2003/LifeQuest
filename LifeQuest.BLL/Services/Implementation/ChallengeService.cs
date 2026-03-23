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
            // هيكرت تحدى جديد
            var challenge = _mapper.Map<Challenge>(dto);
            
            // هنا لو ال DTO مفيهوش قيم معينه، هحط انا ديفولت من عندى او هحسبها
            challenge.CategoryId = dto.CategoryId == 0 ? 1 : dto.CategoryId; // Default to 1 for now if not set
            challenge.Duration = (dto.EndDate - dto.StartDate).Days;
            
            await _unitOfWork.Repository<Challenge>().AddAsync(challenge);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<ChallengeDTO>> GetAllChallengesAsync()
        {
            // هجيب كل التحديات اللى فى السيستم ومعاهم ال Category كمان
            var challenges = await _unitOfWork.Repository<Challenge>().GetAllWithIncludesAsync(c => true, "Category");
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
            _unitOfWork.Repository<Challenge>().Update(challenge);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteChallengeAsync(int id)
        {
            // هسمح للمستخدم يمسح التحدى خالص بال ID بتاعه
            await _unitOfWork.Repository<Challenge>().Delete(id);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ChallengeDTO>> GetChallengesByCategoryAsync(int categoryId)
        {
            // هجيب كل التحديات اللى تبع Category معينة
            var challenges = await _unitOfWork.Repository<Challenge>()
                .GetAllWithIncludesAsync(c => c.CategoryId == categoryId, "Category");
            
            return _mapper.Map<IEnumerable<ChallengeDTO>>(challenges);
        }
    }
}
