using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            // هجيب كل الاقسام اللى عندى
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            // هنا هجيب قسم معين عن طريق ال ID بتاعه
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> CreateCategoryAsync(CategoryDTO dto)
        {
            // هضيف قسم جديد للسيستم
            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Repository<Category>().AddAsync(category);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDTO dto)
        {
            // هعدل بيانات القسم اللى جايلى
            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Repository<Category>().Update(category);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // هحذف القسم خالص من الداتابيز
            await _unitOfWork.Repository<Category>().Delete(id);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
