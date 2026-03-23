using System;

namespace LifeQuest.BLL.DTOs
{
    public class MetricsCalcDTO
    {
        public int Id { get; set; }
        public int SuccessRate { get; set; }
        public int RiskPattern { get; set; }
        public int ConfidenceAccuracy { get; set; }
        public int OverConfidenceIndex { get; set; }
        public int DecisionId { get; set; }
    }
}
