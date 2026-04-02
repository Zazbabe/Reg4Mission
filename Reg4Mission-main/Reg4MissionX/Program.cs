using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Reg4MissionX.Data;
using Reg4MissionX.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// =====================
// SERVICES
// =====================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// IMPORTANT: Identity must use ApplicationUser (not IdentityUser)
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // needed for Areas/Identity pages

var app = builder.Build();

// =====================
// DATABASE INIT
// =====================

try
{
    // Create DB + apply migrations + seed (DEV only)
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        // Use FULL namespace to avoid ambiguous DbInitializer
        await Reg4MissionX.Data.DbInitializer.SeedAsync(services);
    }
}
catch (Exception ex)
{
    // If something fails during init, log it
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Fel vid databasinitering");
}

// =====================
// PIPELINE
// =====================

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Swedish localization
var supportedCultures = new[] { new CultureInfo("sv-SE") };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("sv-SE"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseRouting();

// IMPORTANT: AuthN before AuthZ
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Identity scaffold pages live here: /Identity/Account/Register etc
app.MapRazorPages()
   .WithStaticAssets();

app.Run();