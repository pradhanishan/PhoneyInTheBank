using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
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

            // If Donate To AI is selected then subtract from operative Account
            if (transfer.DonateToAI)
            {
                if (bankAccount.OperativeAmount < 0 || bankAccount.OperativeAmount < transfer.Amount)
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
                    if (bankAccount.InvestmentAmount < 0 || bankAccount.InvestmentAmount < transfer.Amount)
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
            //TODO -[Not So Important] Design Rock Paper Scissor UI
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Earn(RockPaperScissorVM rps)
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
                        ModelState.AddModelError(String.Empty, "Its a tie, you and your opponent both selected rock.");
                        rps.TieFlag = true;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "paper")
                    {
                        ModelState.AddModelError(String.Empty, "You lose, You selected rock, your opponent selected paper.");
                        rps.TieFlag = false;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "scissor")
                    {
                        ModelState.AddModelError(String.Empty, "You win, You selected rock, your opponent selected scissor.");
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
                        ModelState.AddModelError(String.Empty, "You win, You selected paper, your opponent selected rock.");
                        rps.TieFlag = false;
                        rps.VictoryFlag = true;
                    }
                    else if (rps.AIChoice == "paper")
                    {
                        ModelState.AddModelError(String.Empty, "Its a tie, you and your opponent both selected paper.");
                        rps.TieFlag = true;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "scissor")
                    {
                        ModelState.AddModelError(String.Empty, "You lose, You selected paper, your opponent selected scissor.");
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "An Error Occured");
                    }
                    break;
                case "scissor":
                    if (rps.AIChoice == "rock")
                    {
                        ModelState.AddModelError(String.Empty, "You lose, You selected scissor, your opponent selected rock.");
                        rps.TieFlag = false;
                        rps.VictoryFlag = false;
                    }
                    else if (rps.AIChoice == "paper")
                    {
                        ModelState.AddModelError(String.Empty, "You win, You selected scissor, your opponent selected paper.");
                        rps.TieFlag = false;
                        rps.VictoryFlag = true;
                    }
                    else if (rps.AIChoice == "scissor")
                    {
                        ModelState.AddModelError(String.Empty, "Its a tie, you and your opponent both selected scissor.");
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

            if (rps.TieFlag) return View();

            if (!rps.TieFlag && rps.VictoryFlag)
            {
                bankAccount.OperativeAmount += rps.WagerValue;
            }

            if (!rps.TieFlag && !rps.VictoryFlag)
            {
                bankAccount.OperativeAmount -= rps.WagerValue;
            }

            _unitOfWork.BankAccount.Update(bankAccount);
            _unitOfWork.Save();

            return View();
        }

        // Take Loan from AI
        public IActionResult Loan()
        {
            var username = User.Identity?.Name;
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

            IEnumerable<LoanVM> loanTypes = new List<LoanVM>()
            {
                new LoanVM {
                    Id=1,
                    LoanType="Small Loan",
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=25,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/4
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/4)},
                new LoanVM {
                    Id=2,
                    LoanType="Average Loan",
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=50,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/2
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/2)},
                new LoanVM {
                    Id=3,
                    LoanType="Large Loan",
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=100,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount
                    , TotalAmount=3 * bankAccount.OperativeAmount},
            };

            Loan existingLoan = _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);

            if (existingLoan != null)
            {
                return RedirectToAction("PayLoan", "Transaction", new { area = "User" });
            }

            return View(loanTypes);
        }

        // Apply for loan if you do not have an existing loan
        public IActionResult ApplyLoan(int? id)
        {
            var username = User.Identity?.Name;
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

            IEnumerable<LoanVM> loanTypes = new List<LoanVM>()
            {
                new LoanVM {
                    Id=1,
                    LoanType="Small Loan",
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=25,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/4
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/4)},
                new LoanVM {
                    Id=2,
                    LoanType="Average Loan",
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=50,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/2
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/2)},
                new LoanVM {
                    Id=3,
                    LoanType="Large Loan",
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=100,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount
                    , TotalAmount=3 * bankAccount.OperativeAmount},
            };

            LoanVM selectedLoanType = loanTypes.FirstOrDefault(x => x.Id == id);

            Loan loan = new()
            {
                BankAccount = bankAccount,
                LoanAmount = selectedLoanType.LoanAmount,
                LeftToPay = selectedLoanType.LoanAmount,
                InterestRate = selectedLoanType.InterestRate,
                LoanAmountWithInterest = selectedLoanType.TotalAmount,
                LeftToPayWithInterest = selectedLoanType.TotalAmount,

            };

            Loan existingLoan = _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);

            if (existingLoan != null)
            {
                return RedirectToAction("Loan", "User", new { area = "User" });
            }

            bankAccount.OperativeAmount += loan.LoanAmount;
            bankAccount.LoanAmount = loan.LoanAmount;

            _unitOfWork.Loan.Add(loan);
            _unitOfWork.BankAccount.Update(bankAccount);
            _unitOfWork.Save();

            return RedirectToAction("Index", "User", new { area = "User" });
        }

        public IActionResult PayLoan()
        {
            var username = User.Identity?.Name;
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

            Loan existingLoan = _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);

            return View(existingLoan);
        }

        // Get a daily random gift
        public IActionResult Present()
        {
            return View();
        }


    }
}
