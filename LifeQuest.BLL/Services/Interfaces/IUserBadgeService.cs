using LifeQuest.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IUserBadgeService
    {
        public Task<IEnumerable<UserBadgeDTO>> GetUserBadgesAsync(int userId);

        public Task CheckAndAwardBadgesAsync(int userId);
    }
}
