using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Implementation
{
    public class LevelService : ILevelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LevelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LevelDTO>> GetAllLevelsAsync()
        {
            // هجيب كل ال Levels اللى عندى فى السيستم
            var levels = await _unitOfWork.Repository<Level>().GetAllAsync();
            return _mapper.Map<IEnumerable<LevelDTO>>(levels);
        }

        public async Task<LevelDTO?> GetLevelByIdAsync(int id)
        {
            // هبحث عن Level معين بال ID بتاعه
            var level = await _unitOfWork.Repository<Level>().GetByIdAsync(id);
            return _mapper.Map<LevelDTO>(level);
        }

        public async Task<bool> AddLevelAsync(LevelDTO dto)
        {
            // هضيف Level جديد عشان المستخدمين يوصلوله
            var level = _mapper.Map<Level>(dto);
            await _unitOfWork.Repository<Level>().AddAsync(level);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateLevelAsync(LevelDTO dto)
        {
            // هعدل الداتا بتاعة ال Level اللى جايلى
            var level = _mapper.Map<Level>(dto);
            _unitOfWork.Repository<Level>().Update(level);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteLevelAsync(int id)
        {
            // هحذف ال Level ده خالص
            await _unitOfWork.Repository<Level>().Delete(id);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
