using Microsoft.AspNetCore.Mvc;

namespace Reg4MissionX.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        // =========================
        // LOGIN
        // =========================
        // GET:  /Account/Login  ->  /Identity/Account/Login
        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
            => RedirectToIdentityLogin(returnUrl);

        // POST: /Account/Login  ->  /Identity/Account/Login
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPost(string? returnUrl = null)
            => RedirectToIdentityLogin(returnUrl);

        private IActionResult RedirectToIdentityLogin(string? returnUrl)
        {
            var url = "/Identity/Account/Login";
            if (!string.IsNullOrWhiteSpace(returnUrl))
                url += $"?ReturnUrl={Uri.EscapeDataString(returnUrl)}";
            return Redirect(url);
        }

        // =========================
        // REGISTER
        // =========================
        // GET:  /Account/Register -> /Identity/Account/Register
        [HttpGet("Register")]
        public IActionResult Register(string? returnUrl = null)
            => RedirectToIdentityRegister(returnUrl);

        // POST: /Account/Register -> /Identity/Account/Register
        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterPost(string? returnUrl = null)
            => RedirectToIdentityRegister(returnUrl);

        private IActionResult RedirectToIdentityRegister(string? returnUrl)
        {
            var url = "/Identity/Account/Register";
            if (!string.IsNullOrWhiteSpace(returnUrl))
                url += $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
            return Redirect(url);
        }
    }
}