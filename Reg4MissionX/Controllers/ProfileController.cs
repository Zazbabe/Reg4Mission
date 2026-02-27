using Microsoft.AspNetCore.Mvc;

namespace Reg4MissionX.Controllers
{
    public class ProfileController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
    }
}
