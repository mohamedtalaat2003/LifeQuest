using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeQuest.DAL.Models
{
    public class Decision : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public bool IsConfident { get; set; }

        [Required]
        public bool IsSuccess { get; set; }

        public string RiskLevel { get; set; } = "Medium"; // Easy, Medium, Hard

        public MetricsCalc? Metrics { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
