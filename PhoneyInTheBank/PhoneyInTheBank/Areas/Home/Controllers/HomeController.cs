using Microsoft.AspNetCore.Mvc;
using PhoneyInTheBank.Models;
using System.Diagnostics;

namespace PhoneyInTheBank.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return User.Identity.IsAuthenticated ? RedirectToAction("Index", "User", new { Area = "User" }) : View();
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        public IActionResult AboutUs()
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