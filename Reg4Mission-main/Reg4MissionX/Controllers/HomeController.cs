using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reg4MissionX.Models;
using Reg4MissionX.ViewModels;
using System.Diagnostics;

namespace Reg4MissionX.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

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
            var vm = new HomeLoginVm
            {
                ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                    ? "/Dashboard?loggedIn=1"
                    : returnUrl
            };

            return View(vm);
        }

        // POST: /Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(HomeLoginVm model)
        {
            var returnUrl = string.IsNullOrWhiteSpace(model.ReturnUrl)
                ? "/Dashboard?loggedIn=1"
                : model.ReturnUrl;

            if (!ModelState.IsValid)
            {
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