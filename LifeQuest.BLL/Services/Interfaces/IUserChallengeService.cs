using LifeQuest.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IUserChallengeService
    {
        public Task JoinChallengeAsync(int userId, int ChallengeId);   
        Task<IEnumerable<UserChallengeDTO>> GetUserChallengesAsync(int userId);
        public Task<UserChallengeDTO> GetChallengeDetailsAsync(int userId , int challengeId);
    }
}
