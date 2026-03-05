using Microsoft.AspNetCore.Mvc;

namespace Reg4MissionX.Controllers
{
    public class AccountController : Controller
    {
        // Redirect old MVC register route to the real Identity register page
        // GET: /Account/Register  ->  /Identity/Account/Register
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                // Keep ReturnUrl so user can be redirected after successful registration
                return Redirect($"/Identity/Account/Register?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }

            return Redirect("/Identity/Account/Register");
        }

        // If something still POSTs to /Account/Register, redirect to Identity too.
        // (Better than keeping old UI-only logic around.)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterPost(string? returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect($"/Identity/Account/Register?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }

            return Redirect("/Identity/Account/Register");
        }
    }
}