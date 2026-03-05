namespace Reg4MissionX.Models;

public class PrivateProfileMunicipality
{
    public int PrivatePersonProfileId { get; set; }
    public PrivatePersonProfile PrivatePersonProfile { get; set; } = null!;

    public int MunicipalityId { get; set; }
    public Municipality Municipality { get; set; } = null!;
}
