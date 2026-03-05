using System.ComponentModel.DataAnnotations;

namespace Reg4MissionX.ViewModels;

public class ProfileEditVm
{
    // --- Identity user fields ---
    [Display(Name = "Namn")]
    [MaxLength(120)]
    public string FullName { get; set; } = "";

    [Display(Name = "Mailadress")]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = "";

    [Display(Name = "Telefonnummer")]
    [Phone]
    [MaxLength(30)]
    public string? PhoneNumber { get; set; }

    // --- Domain profile fields ---
    [Display(Name = "Ålder")]
    [Range(0, 120)]
    public int? Age { get; set; }

    [Display(Name = "Kön")]
    [MaxLength(40)]
    public string? Gender { get; set; }

    [Display(Name = "LSS")]
    public bool DeptLss { get; set; }

    [Display(Name = "SoL")]
    public bool DeptSol { get; set; }

    [Display(Name = "Socialtjänsten")]
    public bool DeptSocialtjansten { get; set; }

    // --- Municipalities ---
    // selected municipality ids from form
    public List<int> SelectedMunicipalityIds { get; set; } = new();

    // checkbox list in UI
    public List<MunicipalityOptionVm> MunicipalityOptions { get; set; } = new();
}

public class MunicipalityOptionVm
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsChecked { get; set; }
}