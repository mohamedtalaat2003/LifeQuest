using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeQuest.DAL.Models
{
    public class Badges : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Points required to gain this badge
        public int Points { get; set; }

        public string Image { get; set; } = string.Empty;

        // The level required to unlock this badge (nullable for non-level badges)
        [ForeignKey("RequiredLevel")]
        public int? RequiredLevelId { get; set; }
        public Level? RequiredLevel { get; set; }

        public BadgeCriteriaType CriteriaType { get; set; } = BadgeCriteriaType.Level;

        // The count needed for ChallengeCount or Streak criteria
        public int CriteriaValue { get; set; }

        public HashSet<UserBadge> UserBadges { get; set; } = new();
    }
}
