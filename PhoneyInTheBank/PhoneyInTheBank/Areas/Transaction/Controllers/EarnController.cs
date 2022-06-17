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
    public class EarnController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public EarnController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Earn money by playing rock paper scissors
        public IActionResult Index()
        {
            //TODO -[Not So Important] Design Rock Paper Scissor UI
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index(RockPaperScissorVM rps)
        {

            if (!ModelState.IsValid) return View();

            string[] choices = { "rock", "paper", "scissor" };
            Random r = new();
            int choice = r.Next(0, choices.Length);
            rps.AIChoice = choices[choice];


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

            if (bankAccount.OperativeAmount < 0 || bankAccount.OperativeAmount < rps.WagerValue)
            {
                ModelState.AddModelError(string.Empty, "You do not have enough funds to process this requrest.");
                return View();
            }

            switch (rps.UserChoice)
            {
                case "rock":
                    if (rps.AIChoice == "rock")
                    {
                        TempData["Tie"] = "You and your opponent both selected rock.";
                        rps.TieFlag = true;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "paper")
                    {
                        TempData["Loss"] = "You selected rock, your opponent selected paper.";
                        rps.TieFlag = false;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "scissor")
                    {
                        TempData["Victory"] = "You selected rock, your opponent selected scissor.";
                        rps.TieFlag = false;
                        rps.VictoryFlag = true;
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "An Error Occured");
                    }
                    break;
                case "paper":
                    if (rps.AIChoice == "rock")
                    {
                        TempData["Victory"] = "You selected paper, your opponent selected rock.";
                        rps.TieFlag = false;
                        rps.VictoryFlag = true;
                    }
                    else if (rps.AIChoice == "paper")
                    {
                        TempData["Tie"] = "You and your opponent both selected paper.";
                        rps.TieFlag = true;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "scissor")
                    {
                        TempData["Loss"] = "You selected paper, your opponent selected scissor.";
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "An Error Occured");
                    }
                    break;
                case "scissor":
                    if (rps.AIChoice == "rock")
                    {
                        TempData["Loss"] = "You selected scissor, your opponent selected rock.";
                        rps.TieFlag = false;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "paper")
                    {

                        TempData["Victory"] = "You selected scissor, your opponent selected paper.";
                        rps.TieFlag = false;
                        rps.VictoryFlag = true;
                    }
                    else if (rps.AIChoice == "scissor")
                    {
                        TempData["Tie"] = "You and your opponent both selected scissor.";
                        rps.TieFlag = true;
                        rps.VictoryFlag = false;
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "An Error Occured");
                    }
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "Invalid Choice");
                    break;
            }

            TransactionHistory trx = new()
            {
                User = User.Identity.Name,
                SentAmount = rps.WagerValue,
                ReceivedAmount = 0,
                TransactionDate = DateTimeOffset.Now,
                TransactionType = "E",
            };

            if (rps.TieFlag) { }


            if (!rps.TieFlag && rps.VictoryFlag)
            {
                trx.ReceivedAmount = trx.SentAmount;
                trx.SentAmount = 0;
                trx.Message = "Earned " + trx.ReceivedAmount.ToString() + " phonies by playing rock paper scissors.";
                bankAccount.OperativeAmount += rps.WagerValue;
            }

            if (!rps.TieFlag && !rps.VictoryFlag)
            {
                trx.Message = "Lost " + trx.SentAmount.ToString() + " phonies by playing rock paper scissors.";
                bankAccount.OperativeAmount -= rps.WagerValue;
            }

            _unitOfWork.BankAccount.Update(bankAccount);
            _unitOfWork.TransactionHistory.Add(trx);
            _unitOfWork.Save();

            return View();
        }
    }
}
