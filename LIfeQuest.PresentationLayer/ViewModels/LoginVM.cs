using System.ComponentModel.DataAnnotations;

namespace LifeQuest.PL.ViewModels
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string PlainPassword { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
