using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Reg4MissionX.Infrastructure
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            //Roles
            string[] roles = ["Admin", "SysAdmin", "PrivateUser", "TownshipUser"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        //Initialize a user with the specified email, password and role if it doesn't already exist
    private static async Task EnsureUserAsync(
    UserManager<IdentityUser> userManager,
    string email,
    string password,
    string role)
        {
            if (await userManager.FindByEmailAsync(email) != null)
                return;

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Kunde inte skapa användare {email}: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }

            //roles + users can be seeded in any order, so we can call this method for both admin and sysadmin without worrying about role existence
            await EnsureUserAsync(userManager, "admin@test.se", "Admin123!", "Admin");
            await EnsureUserAsync(userManager, "sysadmin@test.se", "SysAdmin123!", "SysAdmin");
        }
        }
    }