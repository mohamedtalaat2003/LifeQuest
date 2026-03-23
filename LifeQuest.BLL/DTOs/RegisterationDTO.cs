using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LifeQuest.BLL.DTOs
{
    public class RegisterationDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string PlainPassword { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public IEnumerable<SelectListItem>? Countries { get; set; }
    }
}
