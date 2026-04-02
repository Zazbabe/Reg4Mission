using System.ComponentModel.DataAnnotations;

namespace Reg4MissionX.ViewModels
{
    public class HomeLoginVm
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Mailadress")]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; } = "";

        [Display(Name = "Kom ihåg mig")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}