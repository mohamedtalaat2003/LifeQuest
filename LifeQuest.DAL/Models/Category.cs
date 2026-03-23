using System.ComponentModel.DataAnnotations;

namespace LifeQuest.DAL.Models
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public HashSet<Challenge> Challenges { get; set; } = new();
    }
}
