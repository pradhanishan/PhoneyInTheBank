using DataContext.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PhoneyInTheBank.Models;
using Repository.UnitOfWork;
using ViewModels;
using Models;

namespace PhoneyInTheBank.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {

            var username = User.Identity?.Name;
            ApplicationUser? user = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);

            if (user == null) return NotFound();

            BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser.Id == user.Id);

            if (bankAccount == null)
            {
                return RedirectToAction("CreateBankAccount", "User", new { area = "User" });
            }

            Score score = await _unitOfWork.Score.GetFirstOrDefault(x => x.ApplicationUser == user);




            UserDashboardVM userVM = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Country = user.Country,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                AccountNumber = bankAccount.AccountNumber,
                OperativeAmount = bankAccount.OperativeAmount,
                LoanAmount = bankAccount.LoanAmount,
                InvestmentAmount = bankAccount.InvestmentAmount,
                BankruptFlag = bankAccount.OperativeAmount ==0 && bankAccount.InvestmentAmount ==0? true:false,
                ActiveUserFlag = user.ActiveFlag,
                ActiveAccountFlag = bankAccount.ActiveFlag,
                UserCreatedDate = user.CreatedDate,
                UserUpdatedDate = user.LastUpdatedDate,
                AccountCreatedDate = bankAccount.CreatedDate,
                AccountUpdatedDate = bankAccount.UpdatedDate,
                LuckScore = score.LuckScore,
                CharityScore = score.GoodWillScore,
                TrustScore = score.TrustScore,
                FinancialStatusScore = score.FinancialScore,

            };

            Present present = await _unitOfWork.Present.GetFirstOrDefault(x => x.ApplicationUser == user);

            if (present == null || present.NextPresentAvailableDate < DateTimeOffset.UtcNow)
            {
                userVM.PresentAvailable = true;
            }

            return View(userVM);
        }

        public async Task<IActionResult> CreateBankAccount()
        {
            var username = User.Identity?.Name;
            ApplicationUser? user = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);

            BankAccount bankAccount = new()
            {
                AccountNumber = _unitOfWork.BankAccount.GenerateAccountNumber(),
                ApplicationUser = user,
            };

            Score score = new()
            {
                ApplicationUser = user,
            };


            await _unitOfWork.BankAccount.Add(bankAccount);
            await _unitOfWork.Score.Add(score);
            await _unitOfWork.Save();
            return RedirectToAction("Index", "User", new { area = "User" });
        }

    }
}
