using Microsoft.AspNetCore.Mvc;

namespace PhoneyInTheBank.Areas.Auth.Controllers
{
    [Area("Auth")]
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            ViewBag.Countries = Utilities.Dropdown.GetCountries();
            return View();
        }
    }
}
