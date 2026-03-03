using Microsoft.AspNetCore.Mvc;

namespace Reg4MissionX.Controllers
{
    public class SearchController : Controller
    {
        // PRIVATE user search UI
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Views/Search/Index.cshtml
        }

        // MUNICIPALITY user search UI
        [HttpGet("/Municipality/Search")]
        public IActionResult Municipality()
        {
            return View("MunicipalityIndex"); // Views/Search/MunicipalityIndex.cshtml
        }

        // MUNICIPALITY: returns the profile modal (UI-only)
        [HttpGet("/Municipality/ProfileModal")]
        public IActionResult MunicipalityProfileModal(int id)
        {
            ViewBag.CandidateId = id; // UI-only placeholder
            return PartialView("_MunicipalityProfileModal");
        }

        // MUNICIPALITY: "Reveal contact info" click (UI only, endpoint exists for tracking later)
        [HttpPost("/Municipality/RevealContact")]
        [ValidateAntiForgeryToken]
        public IActionResult MunicipalityRevealContact(int candidateId)
        {
            // UI-only placeholder:
            // Later: check role/claims, log reveal in DB, then return real contact info partial.
            ViewBag.CandidateId = candidateId;
            return PartialView("_MunicipalityContactInfo");
        }
    }
}