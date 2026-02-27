using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reg4MissionX.ViewModels;
using System.Data;

namespace Reg4MissionX.Controllers
{
    [Authorize(Roles = "Admin,SysAdmin")]
    public class AdminController : Controller
    {
        //Object created for Creating new role in Identity
        private readonly RoleManager<IdentityRole> _roleManager;
        //Object created for Adding user to role in Identity
        private readonly UserManager<IdentityUser> _userManager;

        //Constructor for HomeController to inject RoleManager and UserManager
        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        //Action method to show all users and their roles
        [HttpGet]
        public async Task<IActionResult> GetRoles(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            //Find the user by Id and adds the Id to the variable User.
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            //Get all roles från the database and add them to the variable allRoles. Get the roles for the user and add them to the variable userRoles.
            var allRoles = _roleManager.Roles
                .Select(role => role.Name)
                .OrderBy(role => role)
                .ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            //Create a new instance of the ManageUserRolesVm class and populate it with the user information, current roles, and available roles. Then return the view with the model.
            var model = new ManageUserRolesVm
            {
                UserId = user.Id,
                UserEmail = user.Email,
                CurrentRoles = userRoles.ToList(),
                AvailableRoles = allRoles
            };

            //Return the view with the model
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateRoles()
        {
            return View(new CreateRolesVm());
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoles(CreateRolesVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //Split the input string into individual role names, trim any whitespace, and store them. Where doesnt work here.
            var roles = model.Roles
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(role => role.Trim())
                .ToList();

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            TempData["Success"] = "Roller uppdaterade";

            return RedirectToAction(nameof(Index));
        }

        //Action method to add a user to one or more roles
        [HttpPost]
        public async Task<IActionResult> AddUserToRoles(ManageUserRolesVm model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = _roleManager.Roles.Select(role => role.Name).ToList();
                return View(model);
            }

            //Checks if the CurrentRoles property of the model is null and if it is, it initializes it as an empty list of strings. This ensures that the code can safely work with the CurrentRoles property.
            model.CurrentRoles ??= new List<string>();

            //Finds the user by Id and adds the Id to the variable User
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            //An attribute to store the current roles of the user in the variable currentRoles. The first in the local and the second is from the model. 
            var currentRoles = await _userManager.GetRolesAsync(user);

            //Attributes to store the roles to be added and removed.
            var rolesToAdd = model.CurrentRoles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(model.CurrentRoles);

            //Add the user to the roles to be added and remove the user from the roles to be removed.
            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            TempData["Success"] = "Roller uppdaterade";

            //Redirect to the AddUserToRoles action method with the userId as a parameter to show the updated roles for the user.
            return RedirectToAction(nameof(GetRoles), new { userId = model.UserId });
        }
    }
}
