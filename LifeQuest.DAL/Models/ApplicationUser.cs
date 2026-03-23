using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LifeQuest.DAL.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public UserProfile? UserProfile { get; set; }

        public HashSet<UserBadge> UserBadges { get; set; } = new();
        public HashSet<UserChallenge> UserChallenges { get; set; } = new();
    }
}
