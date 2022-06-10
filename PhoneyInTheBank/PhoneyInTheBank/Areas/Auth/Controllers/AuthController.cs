using DataContext.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhoneyInTheBank.Models;
using ViewModels;

namespace PhoneyInTheBank.Areas.Auth.Controllers
{
    [Area("Auth")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AuthController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            ViewBag.Countries = Utilities.Dropdown.GetCountries();
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM obj)
        {

            ViewBag.Countries = Utilities.Dropdown.GetCountries();
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            if (obj.Password != obj.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                return View(obj);
            }

            var user = new ApplicationUser
            {
                UserName = obj.Email,
                Email = obj.Email,
                City = obj.City,
                PhoneNumber = obj.PhoneNumber,
                Country = obj.Country,
                Age = obj.Age,
            };

            var result = await _userManager.CreateAsync(user, obj.Password);

            if (result.Succeeded)
            {
                // TODO [Important!] -> Email Verification Code

                return RedirectToAction("Index", "User", new { area = "User" });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Login(LoginVM obj)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //ApplicationUser signedUser =await _userManager.FindByEmailAsync(obj.Email);

            var result = await _signInManager.PasswordSignInAsync(obj.Email, obj.Password, obj.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "User", new { area = "User" });
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed To Login.");
                }
                return View();
            }

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }
    }
}
