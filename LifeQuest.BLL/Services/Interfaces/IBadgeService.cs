

using LifeQuest.BLL.DTOs;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IBadgeService
    {
        public Task<IEnumerable<BadgeDTO>> GetAllBadgesAsync();
        Task AddBadgeAsync(BadgeDTO dto);
        Task UpdateBadgeAsync(BadgeDTO dto);
        Task DeleteBadgeAsync(int id);
        public  Task<BadgeDTO> GetBadgeByIdAsync(int id);
        Task<BadgeDTO> GetBadgeByNameAsync(string Name);

    }
}

