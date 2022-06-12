using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneyInTheBank.Models;
using Repository.UnitOfWork;
using ViewModels;

namespace PhoneyInTheBank.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Transfer money to AI or between your Operative and investment Amount 
        public IActionResult Transfer()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public IActionResult Transfer(TransferVM transfer)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var username = User.Identity?.Name;
            if (username == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User");
                return View();
            }
            ApplicationUser user = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User");
                return View();
            }
            BankAccount bankAccount = _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == user);
            if (bankAccount == null)
            {
                ModelState.AddModelError(string.Empty, "This user does not have an active bank account!");
                return View();
            }

            // If Donate To AI is selected, subtract from operative Account
            if (transfer.DonateToAI)
            {
                if (bankAccount.OperativeAmount < 0 || bankAccount.OperativeAmount<transfer.Amount)
				{
                    ModelState.AddModelError(string.Empty, "You do not have enough funds to process this requrest.");
                    return View();
				}
                bankAccount.OperativeAmount -= transfer.Amount;
            }

            // If Donate to AI is not selected, transfer money between Operative and Investment account.
            if (!transfer.DonateToAI)
            {
                if (transfer.TransferType != "oti" && transfer.TransferType != "ito")
                {
                    ModelState.AddModelError(String.Empty, " Invalid Transfer Type");
                    return View();
                }
                if (transfer.TransferType == "oti")
                {
                    if (bankAccount.OperativeAmount < 0 || bankAccount.OperativeAmount < transfer.Amount)
                    {
                        ModelState.AddModelError(string.Empty, "You do not have enough funds to process this requrest.");
                        return View();
                    }
                    bankAccount.OperativeAmount -= transfer.Amount;
                    bankAccount.InvestmentAmount += transfer.Amount;
                }
                if (transfer.TransferType == "ito")
                {
                    if (bankAccount.InvestmentAmount < 0 || bankAccount.OperativeAmount < transfer.Amount)
                    {
                        ModelState.AddModelError(string.Empty, "You do not have enough funds to process this requrest.");
                        return View();
                    }
                    bankAccount.OperativeAmount += transfer.Amount;
                    bankAccount.InvestmentAmount -= transfer.Amount;
                }
            }

            _unitOfWork.BankAccount.Update(bankAccount);
            _unitOfWork.Save();

            return RedirectToAction("Index", "User", new { area = "User" });
        }


        // Earn money by playing rock paper scissors
        public IActionResult Earn()
        {
            return View();
        }

        // Take Loan from AI
        public IActionResult Loan()
        {
            return View();
        }

        // Get a daily random gift
        public IActionResult Present()
        {
            return View();
        }


    }
}
