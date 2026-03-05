using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reg4MissionX.Models;
using Reg4MissionX.ViewModels;
using System.Diagnostics;

namespace Reg4MissionX.Controllers
{
    public class HomeController : Controller
    {
        // Existing: used for creating roles etc.
        private readonly RoleManager<IdentityRole> _roleManager;

        // New: real Identity login
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Inject both
        public HomeController(
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            // Show empty login model for the home login box
            var vm = new HomeLoginVm
            {
                ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/Dashboard" : returnUrl
            };

            return View(vm);
        }

        // POST: /Home/Login (from the login box on Home/Index)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(HomeLoginVm model)
        {
            // Default fallback after login
            var returnUrl = string.IsNullOrWhiteSpace(model.ReturnUrl) ? "/Dashboard" : model.ReturnUrl;

            if (!ModelState.IsValid)
            {
                // Show the same Index page again with validation messages
                model.ReturnUrl = returnUrl;
                return View("Index", model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Fel mailadress eller lösenord.");
            model.ReturnUrl = returnUrl;
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}