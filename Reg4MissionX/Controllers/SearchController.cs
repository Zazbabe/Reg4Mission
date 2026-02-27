using Microsoft.AspNetCore.Mvc;

namespace Reg4MissionX.Controllers
{
    public class SearchController : Controller
    {
        // PRIVATE användare search UI
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Views/Search/Index.cshtml
        }

        // MUNICIPALITY användare search UI
        [HttpGet("/Municipality/Search")]
        public IActionResult Municipality()
        {
            return View("MunicipalityIndex"); // Views/Search/MunicipalityIndex.cshtml
        }

        // MUNICIPALITY: returns the profile modal (UI bara än så länge!)
        [HttpGet("/Municipality/ProfileModal")]
        public IActionResult MunicipalityProfileModal(int id)
        {
            ViewBag.CandidateId = id; // UI-only placeholder
            return PartialView("_MunicipalityProfileModal");
        }

        // MUNICIPALITY: "Reveal contact info" click (UI bara, men endpoint finns för tracking senare)
        [HttpPost("/Municipality/RevealContact")]
        [ValidateAntiForgeryToken]
        public IActionResult MunicipalityRevealContact(int candidateId)
        {
            // UI-only placeholder:
            // Later: check role/claims, log reveal in DB, then return real contact info partial. 
            // blandar lite engelska och svenska haha
            ViewBag.CandidateId = candidateId;
            return PartialView("_MunicipalityContactInfo");
        }
    }
}