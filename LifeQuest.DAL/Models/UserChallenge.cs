using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeQuest.DAL.Models
{
    public class UserChallenge : BaseEntity
    {
        public int UserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public int ChallengeId { get; set; }

        public Challenge? Challenge { get; set; }

        public bool IsSuccess { get; set; }

        public ChallengeStatus Status { get; set; } = ChallengeStatus.NotStarted;

        public int CurrentProgress { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
