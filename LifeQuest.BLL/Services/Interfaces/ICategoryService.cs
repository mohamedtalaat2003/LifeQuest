using LifeQuest.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO?> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(CategoryDTO dto);
        Task<bool> UpdateCategoryAsync(CategoryDTO dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
