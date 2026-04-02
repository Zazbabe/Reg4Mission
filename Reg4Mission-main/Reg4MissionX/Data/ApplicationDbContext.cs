using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reg4MissionX.Models;

namespace Reg4MissionX.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Municipality> Municipalities => Set<Municipality>();
    public DbSet<PrivatePersonProfile> PrivatePersonProfiles => Set<PrivatePersonProfile>();
    public DbSet<PrivateProfileMunicipality> PrivateProfileMunicipalities => Set<PrivateProfileMunicipality>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PrivatePersonProfile>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PrivateProfileMunicipality>()
            .HasKey(x => new { x.PrivatePersonProfileId, x.MunicipalityId });

        builder.Entity<PrivateProfileMunicipality>()
            .HasOne(x => x.PrivatePersonProfile)
            .WithMany(p => p.Municipalities)
            .HasForeignKey(x => x.PrivatePersonProfileId);

        builder.Entity<PrivateProfileMunicipality>()
            .HasOne(x => x.Municipality)
            .WithMany()
            .HasForeignKey(x => x.MunicipalityId);
    }
}