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

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        // 1) Get logged in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        // 2) Load domain profile + municipalities
        var profile = await _context.PrivatePersonProfiles
            .Include(p => p.Municipalities)
            .FirstOrDefaultAsync(p => p.UserId == user.Id);

        // 3) Load all municipalities for checkbox list
        var allMunicipalities = await _context.Municipalities
            .OrderBy(m => m.Name)
            .ToListAsync();

        // 4) Build selected ids set (if profile exists)
        var selectedIds = profile?.Municipalities
            .Select(x => x.MunicipalityId)
            .ToHashSet() ?? new HashSet<int>();

        // 5) Map to Edit VM (if profile is missing, we still show editable form)
        var vm = new ProfileEditVm
        {
            FullName = user.FullName ?? "",
            Email = user.Email ?? user.UserName ?? "",
            PhoneNumber = user.PhoneNumber,

            Age = profile?.Age,
            Gender = profile?.Gender,

            DeptLss = profile?.DeptLss ?? false,
            DeptSol = profile?.DeptSol ?? false,
            DeptSocialtjansten = profile?.DeptSocialtjansten ?? false,

            SelectedMunicipalityIds = selectedIds.ToList(),
            MunicipalityOptions = allMunicipalities.Select(m => new MunicipalityOptionVm
            {
                Id = m.Id,
                Name = m.Name,
                IsChecked = selectedIds.Contains(m.Id)
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileEditVm vm)
    {
        // 1) Get logged in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        // 2) Model validation failed -> reload municipality options so view can render
        if (!ModelState.IsValid)
        {
            var allMunicipalities = await _context.Municipalities
                .OrderBy(m => m.Name)
                .ToListAsync();

            var selected = vm.SelectedMunicipalityIds.ToHashSet();

            vm.MunicipalityOptions = allMunicipalities.Select(m => new MunicipalityOptionVm
            {
                Id = m.Id,
                Name = m.Name,
                IsChecked = selected.Contains(m.Id)
            }).ToList();

            return View(vm);
        }

        // 3) Update Identity user fields
        user.FullName = vm.FullName;
        user.PhoneNumber = vm.PhoneNumber;

        // 4) Email changes in Identity are "special", but for school-project level this works fine
        //    If you want the proper token/confirm flow later, we can upgrade it.
        if (!string.IsNullOrWhiteSpace(vm.Email) && vm.Email != user.Email)
        {
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.NormalizedEmail = vm.Email.ToUpperInvariant();
            user.NormalizedUserName = vm.Email.ToUpperInvariant();
        }

        var userResult = await _userManager.UpdateAsync(user);
        if (!userResult.Succeeded)
        {
            foreach (var err in userResult.Errors)
                ModelState.AddModelError("", err.Description);

            // Reload municipality options so view can render
            var allMunicipalities = await _context.Municipalities
                .OrderBy(m => m.Name)
                .ToListAsync();

            var selected = vm.SelectedMunicipalityIds.ToHashSet();

            vm.MunicipalityOptions = allMunicipalities.Select(m => new MunicipalityOptionVm
            {
                Id = m.Id,
                Name = m.Name,
                IsChecked = selected.Contains(m.Id)
            }).ToList();

            return View(vm);
        }

        // 5) Load domain profile (include municipalities) or create one if missing
        var profile = await _context.PrivatePersonProfiles
            .Include(p => p.Municipalities)
            .FirstOrDefaultAsync(p => p.UserId == user.Id);

        if (profile == null)
        {
            profile = new PrivatePersonProfile
            {
                UserId = user.Id
                // If your model has required fields, set them here too.
            };

            _context.PrivatePersonProfiles.Add(profile);
        }

        // 6) Update domain fields
        profile.Age = vm.Age;
        profile.Gender = vm.Gender;

        profile.DeptLss = vm.DeptLss;
        profile.DeptSol = vm.DeptSol;
        profile.DeptSocialtjansten = vm.DeptSocialtjansten;

        // 7) Update municipalities (clear + re-add)
        //    This assumes your join entity is something like PrivatePersonProfileMunicipality
        //    and your navigation property is profile.Municipalities (List<PrivatePersonProfileMunicipality>)
        profile.Municipalities.Clear();

        var selectedIds = vm.SelectedMunicipalityIds.Distinct().ToList();
        foreach (var municipalityId in selectedIds)
        {
            profile.Municipalities.Add(new PrivateProfileMunicipality
            {
                MunicipalityId = municipalityId
            });
        }

        // 8) Save changes
        await _context.SaveChangesAsync();

        // 9) Redirect back to profile view
        return RedirectToAction(nameof(Index));
    }
}