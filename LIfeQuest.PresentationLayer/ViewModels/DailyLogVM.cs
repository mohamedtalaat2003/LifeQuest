using System.ComponentModel.DataAnnotations;

namespace LifeQuest.PL.ViewModels
{
    public class DailyLogVM
    {
        public int ChallengeId { get; set; }

        [Required]
        [Range(1,1440 , ErrorMessage ="Duration must be between 1 & 1440 minutes .")]
        [Display(Name ="Duration (Minutes")]
        public int Duration { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; } = string.Empty;

    }
}
