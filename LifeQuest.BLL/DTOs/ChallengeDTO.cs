using System;
using LifeQuest.DAL.Models;

namespace LifeQuest.BLL.DTOs
{
    public class ChallengeDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName {  get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Points { get; set; }
        public int Duration { get; set; }
        public bool IsPublic { get; set; } = true;
        public ChallengeDifficulty Difficulty { get; set; } = ChallengeDifficulty.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
        public int ApplicationUserId { get; set; }
    }
}
