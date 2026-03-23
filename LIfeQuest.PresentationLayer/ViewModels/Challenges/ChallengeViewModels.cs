using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LifeQuest.PresentationLayer.ViewModels.Challenges
{
    public class ChallengeViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Points { get; set; }
        public int Duration { get; set; }
        public bool IsPublic { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateChallengeViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start Date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End Date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);

        [Required(ErrorMessage = "Points are required")]
        [Range(1, 1000, ErrorMessage = "Points must be between 1 and 1000")]
        public int Points { get; set; } = 100;

        [Display(Name = "Challenge Difficulty")]
        public string Difficulty { get; set; } = "Medium";

        [Display(Name = "Make Public?")]
        public bool IsPublic { get; set; } = true;
    }
}
