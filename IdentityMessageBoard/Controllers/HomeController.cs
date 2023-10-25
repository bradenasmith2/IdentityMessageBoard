using IdentityMessageBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityMessageBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            //_userManager.GetUserId();

            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public async void AssignUserToAdmin(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user != null)
        //    {
        //        // Check if the 'Admin' role exists
        //        if (await _roleManager.RoleExistsAsync("Admin"))
        //        {
        //            // Add the user to the 'Admin' role
        //            await _userManager.AddToRoleAsync(user, "Admin");
        //        }
        //    }
        //}
    }
}