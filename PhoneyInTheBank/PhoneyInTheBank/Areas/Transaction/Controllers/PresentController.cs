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
    public class PresentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PresentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                Present present = await _unitOfWork.Present.GetFirstOrDefault(x => x.ApplicationUser.Email == User.Identity.Name);
                if (present == null)
                    return RedirectToAction("GetPresent", "Present", new { area = "Transaction" });
                if (DateTime.UtcNow < present.NextPresentAvailableDate)
                    return RedirectToAction("PresentUnavailable", "Present", new { area = "Transaction" });
                return RedirectToAction("GetPresent", "Present", new { area = "Transaction" });
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }

        }

        public async Task<IActionResult> GetPresent()
        {
            try
            {
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

                Score score = await _unitOfWork.Score.GetFirstOrDefault(x => x.ApplicationUser == user);
                if (score == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Score");
                    return View();
                }

                Random r = new();
                int presentNumber = r.Next(0, 99);
                PresentVM presentVM = new();

                Present present = await _unitOfWork.Present.GetFirstOrDefault(x => x.ApplicationUser == user);

                if (present == null)
                {
                    present = new()
                    {
                        ApplicationUser = user,
                    };
                }

                if (DateTime.UtcNow < present.NextPresentAvailableDate)
                {
                    return RedirectToAction("PresentUnavailable", "Present", new { Area = "Transaction" });
                }


                if (presentNumber == 0)
                {
                    presentVM.PresentAmount = CPresentType.LegendaryAmount;
                    presentVM.PresentType = CPresentType.Legendary;
                    _unitOfWork.Score.IncreaseLuck(score, 10);
                }
                if (presentNumber >= 1 && presentNumber <= 10)
                {
                    presentVM.PresentAmount = CPresentType.EpicAmount;
                    presentVM.PresentType = CPresentType.Epic;
                    _unitOfWork.Score.DecreaseLuck(score, 3);
                }
                if (presentNumber >= 11 && presentNumber <= 30)
                {
                    presentVM.PresentAmount = CPresentType.RareAmount;
                    presentVM.PresentType = CPresentType.Rare;
                    _unitOfWork.Score.DecreaseLuck(score, 1);
                }
                if (presentNumber > 30)
                {
                    presentVM.PresentAmount = CPresentType.CommonAmount;
                    presentVM.PresentType = CPresentType.Common;
                }



                DateTimeOffset dt = DateTime.UtcNow;
                present.LastCollectedDate = dt;
                present.NextPresentAvailableDate = dt.AddDays(1);

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == user);
                bankAccount.OperativeAmount += presentVM.PresentAmount;

                if ((presentVM.PresentAmount / bankAccount.OperativeAmount) * 100 >= 5 && (presentVM.PresentAmount / bankAccount.OperativeAmount) * 100 < 5)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 1);
                }
                if ((presentVM.PresentAmount / bankAccount.OperativeAmount) * 100 >= 10 && (presentVM.PresentAmount / bankAccount.OperativeAmount) * 100 < 50)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 3);
                }
                if ((presentVM.PresentAmount / bankAccount.OperativeAmount) * 100 >= 50)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 10);
                }

                TransactionHistory trx = new()
                {
                    ReceivedAmount = presentVM.PresentAmount,
                    TransactionType = CTransaction.TRXTypeGifted,
                    User = User.Identity.Name,
                    Message = CTransaction.GetPresentCollectedLog(presentVM.PresentType.ToString(), presentVM.PresentAmount.ToString())
                };


                _unitOfWork.Present.Update(present);
                _unitOfWork.BankAccount.Update(bankAccount);
                await _unitOfWork.TransactionHistory.Add(trx);
                await _unitOfWork.Save();

                return View(presentVM);
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        public IActionResult PresentUnavailable()
        {
            return View();
        }

    }
}
