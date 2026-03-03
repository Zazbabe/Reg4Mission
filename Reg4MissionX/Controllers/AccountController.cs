using Microsoft.AspNetCore.Mvc;
using Reg4MissionX.ViewModels;

namespace Reg4MissionX.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVm());
        }

        // POST: /Account/Register
        // UI-only: Nothing is saved yet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // UI-only: backend/Identity will be added later
            // For now we just show a success message and redirect back to the form.
            TempData["Success"] = "UI OK! Backend kommer senare.";

            return RedirectToAction(nameof(Register));
        }
    }
}