namespace Reg4MissionX.ViewModels;

public class ProfileViewVm
{
    // Identity user
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }

    // Private profile
    public int? Age { get; set; }
    public string? Gender { get; set; }

    public bool DeptLss { get; set; }
    public bool DeptSol { get; set; }
    public bool DeptSocialtjansten { get; set; }

    public List<string> Municipalities { get; set; } = new();
}

