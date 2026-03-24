using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeQuest.DAL.Models
{
    public class Challenge : BaseEntity
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public ChallengeDifficulty Difficulty { get; set; } = ChallengeDifficulty.Medium;

        [ForeignKey("ApplicationUser")]
        public int ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public HashSet<DailyLog> DailyLogs { get; set; } = new();
    }
}
