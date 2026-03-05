// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reg4MissionX.Data;
using Reg4MissionX.Models;

namespace Reg4MissionX.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Namn är obligatoriskt.")]
            [Display(Name = "Namn")]
            public string FullName { get; set; }

            [Phone(ErrorMessage = "Ogiltigt telefonnummer.")]
            [Display(Name = "Telefonnummer")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Mailadress (inloggning)")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Lösenordet måste vara minst {2} tecken.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Bekräfta lösenord")]
            [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
            public string ConfirmPassword { get; set; }

            [Range(0, 120, ErrorMessage = "Ålder måste vara mellan 0 och 120.")]
            [Display(Name = "Ålder")]
            public int? Age { get; set; }

            [Display(Name = "Kön")]
            public string Gender { get; set; }

            // SCB codes, ex "0114"
            [MinLength(1, ErrorMessage = "Välj minst en kommun.")]
            public List<string> MunicipalityCodes { get; set; } = new();

            // Values: "LSS", "SoL", "Socialtjansten"
            [MinLength(1, ErrorMessage = "Välj minst en avdelning.")]
            public List<string> Departments { get; set; } = new();

            [Range(typeof(bool), "true", "true", ErrorMessage = "Du måste godkänna GDPR-informationen.")]
            [Display(Name = "GDPR")]
            public bool AcceptGdpr { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Extra defensive checks (helpful when checkboxes are involved)
            if (Input.Departments == null || Input.Departments.Count == 0)
                ModelState.AddModelError("Input.Departments", "Välj minst en avdelning.");

            if (Input.MunicipalityCodes == null || Input.MunicipalityCodes.Count == 0)
                ModelState.AddModelError("Input.MunicipalityCodes", "Välj minst en kommun.");

            if (!Input.AcceptGdpr)
                ModelState.AddModelError("Input.AcceptGdpr", "Du måste godkänna GDPR-informationen.");

            if (!ModelState.IsValid)
                return Page();

            // 1) Create Identity user
            var user = CreateUser();
            user.FullName = Input.FullName;
            user.PhoneNumber = Input.PhoneNumber;

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            _logger.LogInformation("User created a new account with password.");

            // 2) Save profile + municipalities + role
            try
            {
                var distinctCodes = Input.MunicipalityCodes.Distinct().ToList();

                var municipalities = await _context.Municipalities
                    .Where(m => distinctCodes.Contains(m.ScbCode))
                    .ToListAsync();

                if (municipalities.Count != distinctCodes.Count)
                {
                    ModelState.AddModelError("Input.MunicipalityCodes", "En eller flera valda kommuner kunde inte hittas i databasen.");
                    await _userManager.DeleteAsync(user); // rollback
                    return Page();
                }

                bool deptLss = Input.Departments.Contains("LSS");
                bool deptSol = Input.Departments.Contains("SoL");
                bool deptSoc = Input.Departments.Contains("Socialtjansten");

                var profile = new PrivatePersonProfile
                {
                    UserId = user.Id,
                    Age = Input.Age,
                    Gender = Input.Gender,

                    DeptLss = deptLss,
                    DeptSol = deptSol,
                    DeptSocialtjansten = deptSoc,

                    GdprAccepted = true,
                    GdprAcceptedAtUtc = DateTime.UtcNow,
                    GdprVersion = "v1"
                };

                foreach (var m in municipalities)
                {
                    profile.Municipalities.Add(new PrivateProfileMunicipality
                    {
                        MunicipalityId = m.Id
                    });
                }

                _context.PrivatePersonProfiles.Add(profile);
                await _context.SaveChangesAsync();

                // Default role for private registrants
                await _userManager.AddToRoleAsync(user, "PrivateUser");
            }
            catch
            {
                // If anything fails, remove created user to keep DB consistent
                await _userManager.DeleteAsync(user);
                throw;
            }

            // 3) Email confirmation (kept from scaffold)
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code, returnUrl },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}