using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reg4MissionX.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity + Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // DEV: allow login without email confirmation
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//
// DEV ONLY: seed roles + one admin user so I can test /Admin directly
//
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = ["Admin", "SysAdmin", "PrivateUser", "TownshipUser"];

    foreach (var r in roles)
    {
        if (!await roleManager.RoleExistsAsync(r))
            await roleManager.CreateAsync(new IdentityRole(r));
    }

    var email = "admin@demo.se";
    var user = await userManager.FindByEmailAsync(email);

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        // Password must match our Identity password rules
        await userManager.CreateAsync(user, "P@ssw0rd123!");
    }

    if (!await userManager.IsInRoleAsync(user, "Admin"))
        await userManager.AddToRoleAsync(user, "Admin");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// IMPORTANT: you must have Authentication BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();