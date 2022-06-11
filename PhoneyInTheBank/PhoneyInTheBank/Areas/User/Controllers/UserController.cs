using DataContext.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PhoneyInTheBank.Models;
using Repository.UnitOfWork;
using ViewModels;

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
        public IActionResult Index()
        {

            var username = User.Identity?.Name;
            ApplicationUser? user = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);

            if (user == null) return NotFound();

            BankAccount bankAccount = _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser.Id == user.Id);

            if (bankAccount == null)
            {
                return RedirectToAction("CreateBankAccount", "User", new { area = "User" });
            }

            UserBankAccountVM userBankAccount = new()
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
                BankruptFlag = bankAccount.BankruptFlag,
                ActiveUserFlag = user.ActiveFlag,
                ActiveAccountFlag = bankAccount.ActiveFlag,
                UserCreatedDate = user.CreatedDate,
                UserUpdatedDate = user.LastUpdatedDate,
                AccountCreatedDate = bankAccount.CreatedDate,
                AccountUpdatedDate = bankAccount.UpdatedDate,
            };

            return View(userBankAccount);
        }

        public IActionResult CreateBankAccount()
        {
            var username = User.Identity?.Name;
            ApplicationUser? user = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);

            BankAccount bankAccount = new()
            {
                AccountNumber = _unitOfWork.BankAccount.GenerateAccountNumber(),
                ApplicationUser = user,
            };
            _unitOfWork.BankAccount.Add(bankAccount);
            _unitOfWork.Save();
            return RedirectToAction("Index", "User", new { area = "User" });
        }

    }
}
