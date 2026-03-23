using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeQuest.DAL.Models
{
    public class UserProfile : BaseEntity
    {
        [Key]
        [ForeignKey("User")] // shared PK
        public int UserId { get; set; }

        [MaxLength(200)]
        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

        // All challenges points
        public int TotalPoints { get; set; }

        // (No.SuccessfulChallenges / TotalChallenges) * 100
        public int SuccessRate { get; set; }

        [ForeignKey("Level")]
        public int LevelId { get; set; }

        public Level Level { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;
    }
}
