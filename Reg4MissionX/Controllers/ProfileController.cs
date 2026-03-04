using Microsoft.AspNetCore.Mvc;

public class ProfileController : Controller
{
    public IActionResult Index()
    {
        // UI-only: later it will load real data from DB / Identity
        return View();
    }

    public IActionResult Edit()
    {
        // UI-only: later it will load real data + postback to save
        return View();
    }
}
