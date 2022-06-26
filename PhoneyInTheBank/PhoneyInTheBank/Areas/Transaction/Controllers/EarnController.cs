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
    public class EarnController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public EarnController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Earn money by playing rock paper scissors
        public async Task<IActionResult> Index()
        {
            try
            {
                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity.Name);

                if (bankAccount.OperativeAmount == 0 && bankAccount.InvestmentAmount == 0)
                {
                    return RedirectToAction("Index", "User", new { Area = "User" });
                }


                ViewData["BankBalance"] = bankAccount.OperativeAmount;

                //TODO -[Not So Important] Design Rock Paper Scissor UI
                return View();
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(RockPaperScissorVM rps)
        {
            try
            {

                if (!ModelState.IsValid) return View();

                string[] choices = { "rock", "paper", "scissor" };
                Random r = new();
                int choice = r.Next(0, choices.Length);
                rps.AIChoice = choices[choice];

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity.Name);

                Score score = await _unitOfWork.Score.GetFirstOrDefault(x => x.ApplicationUser.Email == User.Identity.Name);

                if (bankAccount.OperativeAmount < 0 || bankAccount.OperativeAmount < rps.WagerValue)
                {
                    ModelState.AddModelError(string.Empty, CModelError.InsufficientFundError);
                    return View();
                }

                switch (rps.UserChoice)
                {
                    case "rock":
                        if (rps.AIChoice == CRockPaperScissor.Rock)
                        {
                            TempData["Tie"] = CRockPaperScissor.RockTie;
                            rps.TieFlag = true;
                            rps.VictoryFlag = false;
                        }
                        else if (rps.AIChoice == CRockPaperScissor.Paper)
                        {
                            TempData["Loss"] = CRockPaperScissor.RockLoss;
                            rps.TieFlag = false;
                            rps.VictoryFlag = false;
                        }
                        else if (rps.AIChoice == CRockPaperScissor.Scissor)
                        {
                            TempData["Victory"] = CRockPaperScissor.RockVictory;
                            rps.TieFlag = false;
                            rps.VictoryFlag = true;
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "An Error Occured");
                        }
                        break;
                    case "paper":
                        if (rps.AIChoice == CRockPaperScissor.Rock)
                        {
                            TempData["Victory"] = CRockPaperScissor.PaperVictory;
                            rps.TieFlag = false;
                            rps.VictoryFlag = true;
                        }
                        else if (rps.AIChoice == CRockPaperScissor.Paper)
                        {
                            TempData["Tie"] = CRockPaperScissor.PaperTie;
                            rps.TieFlag = true;
                            rps.VictoryFlag = false;
                        }
                        else if (rps.AIChoice == CRockPaperScissor.Scissor)
                        {
                            TempData["Loss"] = CRockPaperScissor.PaperLoss;
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "An Error Occured");
                        }
                        break;
                    case "scissor":
                        if (rps.AIChoice == CRockPaperScissor.Rock)
                        {
                            TempData["Loss"] = CRockPaperScissor.ScissorLoss;
                            rps.TieFlag = false;
                            rps.VictoryFlag = false;
                        }
                        else if (rps.AIChoice == CRockPaperScissor.Paper)
                        {

                            TempData["Victory"] = CRockPaperScissor.ScissorVictory;
                            rps.TieFlag = false;
                            rps.VictoryFlag = true;
                        }
                        else if (rps.AIChoice == CRockPaperScissor.Scissor)
                        {
                            TempData["Tie"] = CRockPaperScissor.ScissorTie;
                            rps.TieFlag = true;
                            rps.VictoryFlag = false;
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "An Error Occured");
                        }
                        break;
                    default:
                        ModelState.AddModelError(string.Empty, CModelError.InvalidRockPaperScisorChoiceError);
                        break;
                }

                TransactionHistory trx = new()
                {
                    User = User.Identity.Name,
                    SentAmount = rps.WagerValue,
                    ReceivedAmount = 0,
                    TransactionType = CTransaction.TRXTypeEarned,
                };

                if (rps.TieFlag) { }


                if (!rps.TieFlag && rps.VictoryFlag)
                {
                    trx.ReceivedAmount = trx.SentAmount;
                    trx.SentAmount = 0;
                    trx.Message = CTransaction.GetEarnedThroughRPSLog(trx.ReceivedAmount);
                    _unitOfWork.Score.IncreaseLuck(score, 1);

                    if ((rps.WagerValue / bankAccount.OperativeAmount) * 100 >= 5 && (rps.WagerValue / bankAccount.OperativeAmount) * 100 < 5)
                    {
                        _unitOfWork.Score.IncreaseFinancialStatus(score, 1);
                    }
                    if ((rps.WagerValue / bankAccount.OperativeAmount) * 100 >= 10 && (rps.WagerValue / bankAccount.OperativeAmount) * 100 < 50)
                    {
                        _unitOfWork.Score.IncreaseFinancialStatus(score, 3);
                    }
                    if ((rps.WagerValue / bankAccount.OperativeAmount) * 100 >= 50)
                    {
                        _unitOfWork.Score.IncreaseFinancialStatus(score, 10);
                    }

                    bankAccount.OperativeAmount += rps.WagerValue;
                }

                if (!rps.TieFlag && !rps.VictoryFlag)
                {
                    trx.Message = CTransaction.GetLostThroughRPSLog(trx.SentAmount);
                    _unitOfWork.Score.DecreaseLuck(score, 1);

                    if ((rps.WagerValue / bankAccount.OperativeAmount) * 100 >= 5 && (rps.WagerValue / bankAccount.OperativeAmount) * 100 < 5)
                    {
                        _unitOfWork.Score.DecreaseFinancialStatus(score, 1);
                    }
                    if ((rps.WagerValue / bankAccount.OperativeAmount) * 100 >= 10 && (rps.WagerValue / bankAccount.OperativeAmount) * 100 < 50)
                    {
                        _unitOfWork.Score.DecreaseFinancialStatus(score, 3);
                    }
                    if ((rps.WagerValue / bankAccount.OperativeAmount) * 100 >= 50)
                    {
                        _unitOfWork.Score.DecreaseFinancialStatus(score, 10);
                    }

                    bankAccount.OperativeAmount -= rps.WagerValue;
                }

                _unitOfWork.BankAccount.Update(bankAccount);
                await _unitOfWork.TransactionHistory.Add(trx);
                await _unitOfWork.Save();

                return View();
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }
    }
}
