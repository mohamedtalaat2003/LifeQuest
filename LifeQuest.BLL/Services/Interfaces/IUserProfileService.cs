using LifeQuest.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDTO?> GetUserProfileAsync(int userId);

        Task UpdateUserProfileAsync(UserProfileDTO dto);

        Task AddPointsToUserAsync(int userId, int points);

        Task UpdateSuccessRateAsync(int userId);

        Task CheckLevelUpAsync(int userId);
    }
}
