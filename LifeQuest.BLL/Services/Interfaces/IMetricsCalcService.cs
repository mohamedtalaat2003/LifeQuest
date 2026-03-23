using LifeQuest.DAL.Models;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IMetricsCalcService
    {
        Task<MetricsCalc> CalculateUserMetricsAsync(int userId, int decisionId);
    }
}
