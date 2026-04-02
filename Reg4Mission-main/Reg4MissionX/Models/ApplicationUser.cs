using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [MaxLength(120)]
    public string? FullName { get; set; }

    public int? Age { get; set; }

    [MaxLength(40)]
    public string? Gender { get; set; }

    [MaxLength(1000)]
    public string? Interests { get; set; }

    // Optional: if we want to store an image path later
    [MaxLength(500)]
    public string? ProfileImagePath { get; set; }
}