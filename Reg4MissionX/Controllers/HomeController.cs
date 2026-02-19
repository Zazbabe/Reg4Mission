using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reg4MissionX.Models;
using System.Diagnostics;

namespace Reg4MissionX.Controllers
{
    public class HomeController : Controller
    {
        //Created for Creating new role in Identity
        private readonly RoleManager<IdentityRole> RoleManager;

        //Constructor for HomeController to inject RoleManager
        public HomeController(RoleManager<IdentityRole> roleManager)
        {
            RoleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
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
