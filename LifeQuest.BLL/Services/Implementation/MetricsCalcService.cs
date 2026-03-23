using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Implementation
{
    public class MetricsCalcService : IMetricsCalcService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MetricsCalcService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MetricsCalc> CalculateUserMetricsAsync(int userId, int decisionId)
        {
            // هجيب كل القرارات اللى المستخدم عملها
            var allDecisions = await _unitOfWork.Repository<Decision>()
                .GetAllWithIncludesAsync(d => d.UserId == userId);

            // هنركز بس على القرارات اللى خلصت (يعنى نتيجتها مش null)
            var resolvedDecisions = allDecisions.Where(d => d.IsSuccess.HasValue).ToList();

            if (!resolvedDecisions.Any())
            {
                return new MetricsCalc { DecisionId = decisionId };
            }

            int totalResolved = resolvedDecisions.Count();
            int successfulDecisions = resolvedDecisions.Count(d => d.IsSuccess == true);
            
            // 1️⃣ Overall Success Rate (among resolved only)
            int successRate = (int)((double)successfulDecisions / totalResolved * 100);

            // 2️⃣ Confidence Accuracy
            var confidentDecisions = resolvedDecisions.Where(d => d.IsConfident == true);
            int confidenceAccuracy = 0;
            if (confidentDecisions.Any())
            {
                int successfulConfident = confidentDecisions.Count(d => d.IsSuccess == true);
                confidenceAccuracy = (int)((double)successfulConfident / confidentDecisions.Count() * 100);
            }

            // 3️⃣ Over-Confidence Index
            int failedConfident = confidentDecisions.Count(d => d.IsSuccess == false);
            int overConfidenceIndex = (int)((double)failedConfident / totalResolved * 100);

            // 4️⃣ Risk Pattern
            int hardDecisions = allDecisions.Count(d => d.RiskLevel == "Hard");
            int riskPattern = (int)((double)hardDecisions / allDecisions.Count() * 100);

            return new MetricsCalc
            {
                DecisionId = decisionId,
                SuccessRate = successRate,
                ConfidenceAccuracy = confidenceAccuracy,
                OverConfidenceIndex = overConfidenceIndex,
                RiskPattern = riskPattern,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now
            };
        }
    }
}
