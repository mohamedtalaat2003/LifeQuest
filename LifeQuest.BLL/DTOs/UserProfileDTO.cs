using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.BLL.DTOs
{
    public class UserProfileDTO
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string LevelName { get; set; } = string.Empty;

        public int LevelNumber { get; set; }

        public int TotalPoints { get; set; }

        public int SuccessRate { get; set; }

        public int TotalBadges { get; set; }

        public int ActiveChallenges { get; set; }

        public int RemainingPointsForNextLevel { get; set; }

    }

}
