using System.ComponentModel.DataAnnotations;

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
        public HashSet<UserBadge> UserBadges { get; set; } = new();
    }
}
