using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using PhoneyInTheBank.Models;
using Repository.UnitOfWork;
using ViewModels;

namespace PhoneyInTheBank.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class TransferController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransferController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Index(TransferVM transfer)
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
            ApplicationUser user = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User");
                return View();
            }
            BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == user);
            if (bankAccount == null)
            {
                ModelState.AddModelError(string.Empty, "This user does not have an active bank account!");
                return View();
            }


            TransactionHistory trx = new()
            {
                User = User.Identity.Name,
                SentAmount = transfer.Amount,
                ReceivedAmount = 0,
                TransactionDate = DateTimeOffset.Now,
            };


            // If Donate To AI is selected then subtract from operative Account
            if (transfer.DonateToAI)
            {
                if (bankAccount.OperativeAmount < 0 || bankAccount.OperativeAmount < transfer.Amount)
                {
                    ModelState.AddModelError(string.Empty, "You do not have enough funds to process this requrest.");
                    return View();
                }
                trx.TransactionType = "D";
                trx.Message = "Donated " + transfer.Amount.ToString() + " phonies to AI";
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
                    trx.TransactionType = "OI";
                    trx.Message = "Transferred " + transfer.Amount.ToString() + " phonies from operative to investment account.";
                    bankAccount.OperativeAmount -= transfer.Amount;
                    bankAccount.InvestmentAmount += transfer.Amount;
                }
                if (transfer.TransferType == "ito")
                {
                    if (bankAccount.InvestmentAmount < 0 || bankAccount.InvestmentAmount < transfer.Amount)
                    {
                        ModelState.AddModelError(string.Empty, "You do not have enough funds to process this requrest.");
                        return View();
                    }
                    trx.TransactionType = "IO";
                    trx.Message = "Transferred " + transfer.Amount.ToString() + " phonies from investment to operative account.";
                    bankAccount.OperativeAmount += transfer.Amount;
                    bankAccount.InvestmentAmount -= transfer.Amount;
                }
            }

            _unitOfWork.BankAccount.Update(bankAccount);
            await _unitOfWork.TransactionHistory.Add(trx);
            await _unitOfWork.Save();
            TempData["Success"] = "Transfer complete.";

            return RedirectToAction("Index", "User", new { area = "User" });
        }

    }
}
