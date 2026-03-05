using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reg4MissionX.Data;
using Reg4MissionX.Models;
using Reg4MissionX.ViewModels;

namespace Reg4MissionX.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // 1) Get logged in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        // 2) Load domain profile + municipalities
        var profile = await _context.PrivatePersonProfiles
            .Include(p => p.Municipalities)
                .ThenInclude(pm => pm.Municipality)
            .FirstOrDefaultAsync(p => p.UserId == user.Id);

        // 3) If profile missing (user exists but didn’t create profile)
        if (profile == null)
        {
            // We can choose: redirect to register-completion page later.
            // For now: show an empty profile with user basics.
            var emptyVm = new ProfileViewVm
            {
                FullName = user.FullName ?? "",
                Email = user.Email ?? user.UserName ?? "",
                PhoneNumber = user.PhoneNumber
            };

            return View(emptyVm);
        }

        // 4) Map to VM
        var vm = new ProfileViewVm
        {
            FullName = user.FullName ?? "",
            Email = user.Email ?? user.UserName ?? "",
            PhoneNumber = user.PhoneNumber,

            Age = profile.Age,
            Gender = profile.Gender,

            DeptLss = profile.DeptLss,
            DeptSol = profile.DeptSol,
            DeptSocialtjansten = profile.DeptSocialtjansten,

            Municipalities = profile.Municipalities
                .Select(x => x.Municipality.Name)
                .OrderBy(x => x)
                .ToList()
        };

        return View(vm);
    }
}
