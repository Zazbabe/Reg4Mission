using Microsoft.AspNetCore.Identity;

namespace Reg4MissionX.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}