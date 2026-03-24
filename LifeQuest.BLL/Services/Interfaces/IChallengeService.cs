using LifeQuest.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IChallengeService
    {
        public Task CreateChallengeAsync(ChallengeDTO dto);
        public Task<List<ChallengeDTO>> GetAllChallengesAsync(int? userId = null, bool isAdmin = false);
        public Task<ChallengeDTO?> GetChallengeByIdAsync(int id);
        public Task<bool> UpdateChallengeAsync(ChallengeDTO dto);
        public Task<bool> DeleteChallengeAsync(int id);
        public Task<IEnumerable<ChallengeDTO>> GetChallengesByCategoryAsync(int categoryId, int? userId = null);
    }
}
