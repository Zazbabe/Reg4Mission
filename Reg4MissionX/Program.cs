using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reg4MissionX.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Reg4MissionX.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// =====================
// SERVICES
// =====================

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


// =====================
// DATABASE INIT
// =====================

// AUTO: create DB + tables + seed data on app startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); //this will create the database and apply any pending migrations

        // 🔐 SAFETY CHECK: run seeding only in development environment to avoid accidentally creating users/roles in production
        if (app.Environment.IsDevelopment())
        {
            await DbInitializer.SeedAsync(services);
        }    
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Fel vid databasinitering");
    }
}

// =====================
// PIPELINE
// =====================

//

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
var supportedCultures = new[] { new CultureInfo("sv-SE") };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("sv-SE"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

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