using LifeQuest.BLL.DTOs;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IDailyLogService
    {
        Task<bool> AddLogAsync(DailyLogDTO dailyLogDto, int userId);
        public Task<IEnumerable<DailyLogDTO>> GetLogsByDateAsync(int userId , DateTime date);
        public Task<double> GetChallengeProcessPrecentageAsync(int userChallengeId);
        public Task<int> GetUserSteakAsync (int userId);
    }
}
