using Microsoft.AspNetCore.Mvc;

namespace Reg4MissionX.Controllers
{
    public class SearchController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
