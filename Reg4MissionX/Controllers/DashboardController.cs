using Microsoft.AspNetCore.Mvc;

namespace Reg4MissionX.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
    }
}
