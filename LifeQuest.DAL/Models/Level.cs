namespace LifeQuest.DAL.Models
{
    public class Level : BaseEntity
    {
        public int LevelsCount { get; set; }

        public string LevelName { get; set; } = string.Empty;

        // Points required to gain this level
        public int Point { get; set; }

    }
}
