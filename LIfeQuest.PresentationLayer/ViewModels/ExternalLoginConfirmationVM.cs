using System.ComponentModel.DataAnnotations;

namespace LifeQuest.PL.ViewModels
{
    public class ExternalLoginConfirmationVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
