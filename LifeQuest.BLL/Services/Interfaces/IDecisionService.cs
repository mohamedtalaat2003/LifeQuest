using LifeQuest.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Interfaces
{
    public interface IDecisionService
    {
        Task<bool> AddDecisionAsync(DecisionDTO dto);
        Task<bool> UpdateDecisionResultAsync(int decisionId, bool isSuccess);
        Task<IEnumerable<DecisionDTO>> GetUserDecisionsAsync(int userId);
        Task<DecisionDTO?> GetDecisionDetailsAsync(int decisionId);
        Task<bool> DeleteDecisionAsync(int decisionId);
    }
}
