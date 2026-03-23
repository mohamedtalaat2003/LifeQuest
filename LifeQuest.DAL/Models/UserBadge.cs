using System.ComponentModel.DataAnnotations.Schema;

namespace LifeQuest.DAL.Models
{
    public class UserBadge : BaseEntity
    {
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public int BadgeId { get; set; }

        [ForeignKey("BadgeId")]
        public Badges? Badge { get; set; }

        public DateTime AwardedDate { get; set; } = DateTime.Now;
    }
}
