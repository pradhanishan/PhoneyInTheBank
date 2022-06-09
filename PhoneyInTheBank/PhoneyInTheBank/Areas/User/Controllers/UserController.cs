using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PhoneyInTheBank.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
