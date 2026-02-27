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
        // (UI-only: INGET SPARAS ÄNNU)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // UI-only: här kommer backend/Identity senare! :)
            // För nu kan vi bara visa en “tack”-ruta eller skicka tillbaka.
            TempData["Message"] = "UI OK! Backend kommer senare";
            return RedirectToAction(nameof(Register));
        }
    }
}
