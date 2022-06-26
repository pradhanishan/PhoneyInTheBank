using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using PhoneyInTheBank.Models;
using Repository.UnitOfWork;
using Utilities;
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

        public async Task<IActionResult> Index()
        {
            try
            {
                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity.Name);

                if (bankAccount.OperativeAmount == 0 && bankAccount.InvestmentAmount == 0)
                {
                    return RedirectToAction("Index", "User", new { Area = "User" });
                }
                return View();
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Index(TransferVM transfer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity?.Name);

                if (bankAccount.OperativeAmount == 0 && bankAccount.InvestmentAmount == 0)
                {
                    return RedirectToAction("Index", "User", new { Area = "User" });
                }

                Score score = await _unitOfWork.Score.GetFirstOrDefault(x => x.ApplicationUser.Email == User.Identity.Name);


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
                        ModelState.AddModelError(string.Empty, CModelError.InvalidTransferTypeError);
                        return View();
                    }

                    if ((transfer.Amount / bankAccount.OperativeAmount) * 100 >= 5 && (transfer.Amount / bankAccount.OperativeAmount) * 100 < 10)
                    {
                        _unitOfWork.Score.IncreaseGoodwill(score, 1);
                        _unitOfWork.Score.DecreaseFinancialStatus(score, 1);
                    }
                    if ((transfer.Amount / bankAccount.OperativeAmount) * 100 >= 10 && (transfer.Amount / bankAccount.OperativeAmount) * 100 < 50)
                    {
                        _unitOfWork.Score.IncreaseGoodwill(score, 3);
                        _unitOfWork.Score.DecreaseFinancialStatus(score, 3);
                    }
                    if ((transfer.Amount / bankAccount.OperativeAmount) * 100 >= 50)
                    {
                        _unitOfWork.Score.IncreaseGoodwill(score, 10);
                        _unitOfWork.Score.DecreaseFinancialStatus(score, 10);
                    }

                    trx.TransactionType = CTransaction.TRXTypeDonate;
                    trx.Message = CTransaction.GetDonatedLog(transfer.Amount);
                    bankAccount.OperativeAmount -= transfer.Amount;


                }


                // If Donate to AI is not selected, transfer money between Operative and Investment account.
                if (!transfer.DonateToAI)
                {
                    if (transfer.TransferType != CTransferType.OperativeToInvestment && transfer.TransferType != CTransferType.InvestmentToOperative)
                    {
                        ModelState.AddModelError(String.Empty, CModelError.InvalidTransferTypeError);
                        return View();
                    }
                    if (transfer.TransferType == CTransferType.OperativeToInvestment)
                    {
                        if (bankAccount.OperativeAmount < 0 || bankAccount.OperativeAmount < transfer.Amount)
                        {
                            ModelState.AddModelError(string.Empty, CModelError.InsufficientFundError);
                            return View();
                        }
                        trx.TransactionType = "OI";
                        trx.Message = CTransaction.GetOperativeToInvestmentTransferLog(transfer.Amount);
                        bankAccount.OperativeAmount -= transfer.Amount;
                        bankAccount.InvestmentAmount += transfer.Amount;
                    }
                    if (transfer.TransferType == CTransferType.InvestmentToOperative)
                    {
                        if (bankAccount.InvestmentAmount < 0 || bankAccount.InvestmentAmount < transfer.Amount)
                        {
                            ModelState.AddModelError(string.Empty, CModelError.InsufficientFundError);
                            return View();
                        }
                        trx.TransactionType = CTransaction.TRXTypeInvestmentToOperativeTransfer;
                        trx.Message = CTransaction.GetInvestmentToOperativeTransferLog(transfer.Amount);
                        bankAccount.OperativeAmount += transfer.Amount;
                        bankAccount.InvestmentAmount -= transfer.Amount;
                    }
                }

                _unitOfWork.BankAccount.Update(bankAccount);
                await _unitOfWork.TransactionHistory.Add(trx);
                await _unitOfWork.Save();
                TempData["Success"] = CTransaction.TransferComplete;

                return RedirectToAction("Index", "User", new { area = "User" });
            }


            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

    }
}
