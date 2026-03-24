using System;
using LifeQuest.DAL.Models;

namespace LifeQuest.BLL.DTOs
{
    public class UserChallengeDTO
    {
        public int Id { get; set; }

        public int ChallengeId { get; set; }

        public int UserId { get; set; }

        public string ChallengeName { get; set; }

        public ChallengeStatus Status { get; set; }

        public bool IsSuccess { get; set; }

        public int CurrentProgress { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
