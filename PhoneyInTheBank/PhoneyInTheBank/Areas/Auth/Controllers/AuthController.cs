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
            try
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
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
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

                    TempData["Success"] = "Registered successfully, Login to continue.";
                    return RedirectToAction("Index", "User", new { area = "User" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    TempData["Success"] = "Registered failed.";
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                //ApplicationUser signedUser =await _userManager.FindByEmailAsync(obj.Email);

                var result = await _signInManager.PasswordSignInAsync(obj.Email, obj.Password, obj.RememberMe, false);


                if (result.Succeeded)
                {
                    TempData["Success"] = "Welcome " + obj.Email;
                    return RedirectToAction("Index", "User", new { area = "User" });
                }
                else
                {
                    if (result.IsLockedOut)
                    {
                        TempData["Error"] = "Your email is locked.";
                        ModelState.AddModelError("Login", "You are locked out.");
                    }
                    else
                    {
                        TempData["Error"] = "Invalid login credentials.";
                        ModelState.AddModelError("Login", "Failed To Login.");
                    }
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "Logged out successfully";
                return RedirectToAction("Index", "Home", new { area = "Home" });
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }

        }
    }
}
