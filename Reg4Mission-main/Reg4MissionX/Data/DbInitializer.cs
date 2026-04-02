using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reg4MissionX.Models;

namespace Reg4MissionX.Data;

public static class DbInitializer
{
    private class Root { public List<County> Counties { get; set; } = new(); }
    private class County
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public List<Muni> Municipalities { get; set; } = new();
    }
    private class Muni { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        await context.Database.MigrateAsync();

        // Roles
        string[] roles = ["Admin", "SysAdmin", "PrivateUser", "MunicipalityUser"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // SysAdmin seed
        const string adminEmail = "sysadmin@test.se";
        const string adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "SysAdmin"
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "SysAdmin");
        }

        // Municipalities seed from JSON
        if (!await context.Municipalities.AnyAsync())
        {
            var path = Path.Combine(env.WebRootPath, "data", "sweden_counties_municipalities_2026.json");
            var json = await File.ReadAllTextAsync(path);

            var root = JsonSerializer.Deserialize<Root>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new Root();

            var list = new List<Municipality>();

            foreach (var county in root.Counties)
            {
                foreach (var m in county.Municipalities)
                {
                    list.Add(new Municipality
                    {
                        Name = m.Name,
                        ScbCode = m.Code,
                        CountyCode = county.Code,
                        CountyName = county.Name
                    });
                }
            }

            context.Municipalities.AddRange(list);
            await context.SaveChangesAsync();
        }
    }
}