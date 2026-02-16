using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reg4MissionX.ViewModels
{
    public class RegisterVm
    {
       
        [Required(ErrorMessage = "Fyll i namn.")]
        [Display(Name = "Namn")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Fyll i en giltig mailadress.")]
        [EmailAddress]
        [Display(Name = "Mailadress (inloggning)")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Fyll i telefonnummer.")]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "Fyll i lösenord.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken.")]
        [Display(Name = "Lösenord")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Bekräfta lösenord.")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare(nameof(Password), ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; } = "";

        // PRIVATPERSON
        [Required(ErrorMessage = "Fyll i ålder.")]
        [Range(0, 120, ErrorMessage = "Ålder måste vara mellan 0 och 120.")]
        [Display(Name = "Ålder")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Välj kön.")]
        [Display(Name = "Kön")]
        public string Gender { get; set; } = "";

        [Required(ErrorMessage = "Välj län.")]
        [Display(Name = "Län")]
        public string CountyCode { get; set; } = "";

        // Multi-select
        [MinLength(1, ErrorMessage = "Välj minst en kommun.")]
        public List<string> MunicipalityCodes { get; set; } = new();

        [MinLength(1, ErrorMessage = "Välj minst en avdelning.")]
        public List<string> Departments { get; set; } = new(); // "LSS", "SoL", "Socialtjänsten"

        // GDPR checkbox: Required funkar inte “bra” på bool (bool är alltid true/false).
        // Bästa sättet är Range(true,true)
        [Range(typeof(bool), "true", "true", ErrorMessage = "Du måste godkänna GDPR.")]
        public bool AcceptGdpr { get; set; }
    }
}
