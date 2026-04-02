namespace Reg4MissionX.Models;

public class PrivatePersonProfile
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;

    public int? Age { get; set; }
    public string? Gender { get; set; }

    public bool DeptLss { get; set; }
    public bool DeptSol { get; set; }
    public bool DeptSocialtjansten { get; set; }

    public bool GdprAccepted { get; set; }
    public DateTime GdprAcceptedAtUtc { get; set; }
    public string GdprVersion { get; set; } = "v1";

    public ICollection<PrivateProfileMunicipality> Municipalities { get; set; } = new List<PrivateProfileMunicipality>();
}