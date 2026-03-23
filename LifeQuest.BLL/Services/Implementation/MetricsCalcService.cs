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
            // هجيب كل القرارات اللى المستخدم عملها عشان نحللها
            var decisions = await _unitOfWork.Repository<Decision>()
                .GetAllWithIncludesAsync(d => d.UserId == userId);

            if (!decisions.Any())
            {
                return new MetricsCalc { DecisionId = decisionId };
            }

            int totalDecisions = decisions.Count();
            int successfulDecisions = decisions.Count(d => d.IsSuccess);
            
            // 1️⃣ نسبة النجاح العامة
            int successRate = (int)((double)successfulDecisions / totalDecisions * 100);

            // 2️⃣ دقة الثقة (هل لما بيبقى واثق بيطلع صح فعلاً؟)
            var confidentDecisions = decisions.Where(d => d.IsConfident);
            int confidenceAccuracy = 0;
            if (confidentDecisions.Any())
            {
                int successfulConfident = confidentDecisions.Count(d => d.IsSuccess);
                confidenceAccuracy = (int)((double)successfulConfident / confidentDecisions.Count() * 100);
            }

            // 3️⃣ مؤشر الثقة الزايدة (كام مرة كان واثق وفشل؟)
            int failedConfident = confidentDecisions.Count(d => !d.IsSuccess);
            int overConfidenceIndex = (int)((double)failedConfident / totalDecisions * 100);

            // 4️⃣ نمط المخاطرة
            // هنا بنحسب هو بيختار قرارات صعبة (Hard) بنسبة قد إيه
            int hardDecisions = decisions.Count(d => d.RiskLevel == "Hard");
            int riskPattern = (int)((double)hardDecisions / totalDecisions * 100);

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
