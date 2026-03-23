namespace LifeQuest.BLL.DTOs
{
    public class DailyLogDTO
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public int Points { get; set; }
        public int CurrentProgress { get; set; }
        public int ChallengeId { get; set; }
        public string? ChallengeName { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime LogDate { get; set; } = DateTime.Now;
    }
}
