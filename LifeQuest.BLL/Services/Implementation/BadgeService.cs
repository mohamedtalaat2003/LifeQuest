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
    public class BadgeService : IBadgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BadgeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddBadgeAsync(BadgeDTO dto)
        {
            // اضافه وسام جديد
            var badge = _mapper.Map<Badges>(dto);
            await _unitOfWork.Repository<Badges>().AddAsync(badge);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteBadgeAsync(int id)
        {
            // حذف وسام
            await _unitOfWork.Repository<Badges>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<BadgeDTO>> GetAllBadgesAsync()
        {
            // جلب كل الاوسمه مع الليفل المطلوب بتاعها
            var badges = await _unitOfWork.Repository<Badges>().GetAllWithIncludesAsync(null, "RequiredLevel");
            return _mapper.Map<IEnumerable<BadgeDTO>>(badges);
        }

        public async Task<BadgeDTO> GetBadgeByIdAsync(int id)
        {
            // جلب وسام معين بال ID
            var badge = await _unitOfWork.Repository<Badges>().GetByIdAsync(id);
            return _mapper.Map<BadgeDTO>(badge);
        }
        public async Task<BadgeDTO> GetBadgeByNameAsync(string Name)
        {
            // جلب وسام معين بال ID
            var badge = await _unitOfWork.Repository<Badges>().GetByNameAsync(Name);
            
            return _mapper.Map<BadgeDTO>(badge);
        }

        public async Task UpdateBadgeAsync(BadgeDTO dto)
        {
            // تحديث بيانات الوسام
            var badge = _mapper.Map<Badges>(dto);
            _unitOfWork.Repository<Badges>().Update(badge);
            await _unitOfWork.CompleteAsync();
        }
    }
}
