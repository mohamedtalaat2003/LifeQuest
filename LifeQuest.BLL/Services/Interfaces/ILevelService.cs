using LifeQuest.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface ILevelService
    {
        Task<IEnumerable<LevelDTO>> GetAllLevelsAsync();
        Task<LevelDTO?> GetLevelByIdAsync(int id);
        Task<bool> AddLevelAsync(LevelDTO dto);
        Task<bool> UpdateLevelAsync(LevelDTO dto);
        Task<bool> DeleteLevelAsync(int id);
    }
}
