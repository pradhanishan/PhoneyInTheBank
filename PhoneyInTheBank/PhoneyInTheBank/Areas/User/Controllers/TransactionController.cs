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
            _unitOfWork.TransactionHistory.Add(trx);
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

            TransactionHistory trx = new()
            {
                User = User.Identity.Name,
                SentAmount = rps.WagerValue,
                ReceivedAmount = 0,
                TransactionDate = DateTimeOffset.Now,
                TransactionType = "E",
            };

            if (rps.TieFlag) return View();

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
            bankAccount.LoanAmount = loan.LoanAmountWithInterest;

            _unitOfWork.Loan.Add(loan);
            _unitOfWork.BankAccount.Update(bankAccount);

            TransactionHistory trx = new()
            {
                ReceivedAmount = selectedLoanType.LoanAmount,
                TransactionType = "LT",
                SentAmount = 0,
                User = User.Identity.Name,
                Message = "Took a " + selectedLoanType.LoanType + " of " + selectedLoanType.LoanAmount.ToString() + " phonies ",
            };
            _unitOfWork.TransactionHistory.Add(trx);
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

            LoanPaymentVM loanPayment = new()
            {
                AmountTaken = existingLoan.LoanAmount,
                InterestRate = existingLoan.InterestRate,
                AmountWithInterest = existingLoan.LoanAmountWithInterest,
                AmountLeftToPay = existingLoan.LeftToPayWithInterest,
                TakenOn = existingLoan.CreatedDate,
            };

            return View(loanPayment);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult PayLoan(LoanPaymentVM loanPayment)
        {

            if (!ModelState.IsValid)
            {
                return View(loanPayment);
            }

            var username = User.Identity?.Name;
            ApplicationUser user = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User");
                return View(loanPayment);
            }
            BankAccount bankAccount = _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == user);
            if (bankAccount == null)
            {
                ModelState.AddModelError(string.Empty, "This user does not have an active bank account!");
                return View(loanPayment);
            }

            Loan existingLoan = _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);

            if (loanPayment.ClearanceAmount > existingLoan.LeftToPayWithInterest)
            {

                ModelState.AddModelError(string.Empty, "You are paying more than you owe!");
                return View(loanPayment);


            }
            if (bankAccount.OperativeAmount < loanPayment.ClearanceAmount)
            {
                ModelState.AddModelError(string.Empty, "You do not have enough funds to pay");
                return View(loanPayment);
            }

            bankAccount.OperativeAmount -= loanPayment.ClearanceAmount;
            bankAccount.LoanAmount -= loanPayment.ClearanceAmount;
            existingLoan.LeftToPayWithInterest -= loanPayment.ClearanceAmount;
            loanPayment.AmountLeftToPay -= loanPayment.ClearanceAmount;

            if (existingLoan.LeftToPayWithInterest == 0)
            {
                existingLoan.ActiveFlag = false;
            }
            existingLoan.UpdatedDate = DateTimeOffset.Now;

            TransactionHistory trx = new()
            {
                SentAmount = loanPayment.ClearanceAmount,
                TransactionType = "LG",
                ReceivedAmount = 0,
                User = User.Identity.Name,
                Message = "Paid " + loanPayment.ClearanceAmount.ToString() + " phonies from loan of " + loanPayment.AmountWithInterest.ToString(),
            };

            _unitOfWork.BankAccount.Update(bankAccount);
            _unitOfWork.Loan.Update(existingLoan);
            _unitOfWork.TransactionHistory.Add(trx);
            _unitOfWork.Save();

            return View(loanPayment);
        }

        // Get a daily random gift
        public IActionResult Present()
        {
            return View();
        }

        public IActionResult History(int startingNumber, string direction)
        {

            int numberOfTransactions = _unitOfWork.TransactionHistory.GetUserTransactions(x => x.User == User.Identity.Name).Count();

            if (direction == "prev")
            {
                if (startingNumber - 5 <= 0)
                {
                    startingNumber = 0;
                }
                else
                {
                    startingNumber -= 5;
                }

            }

            if (direction == "next")
            {
                if (startingNumber >= numberOfTransactions)
                {
                    startingNumber -= numberOfTransactions - 5;
                }
                else if (startingNumber < numberOfTransactions)
                {
                    if (numberOfTransactions - startingNumber > 5)
                    {
                        startingNumber += 5;
                    }
                }
            }

            ViewData["StartingNumber"] = startingNumber;
            ViewData["CurrentPage"] = startingNumber / 5 + 1;
            ViewData["TotalPages"] = numberOfTransactions%5==0 ?(numberOfTransactions / 5) : (numberOfTransactions / 5) + 1;
            
            


            IEnumerable<TransactionHistory> transactions = _unitOfWork.TransactionHistory.GetUserTransactions(x => x.User == User.Identity.Name).OrderBy(x => x.TransactionDate).Reverse().Skip(startingNumber).Take(5);
            List<TransactionHistoryVM> transactionsVM = new List<TransactionHistoryVM>();
            foreach (var transaction in transactions)
            {
                TransactionHistoryVM vm = new()
                {
                    ReceivedAmount = transaction.ReceivedAmount,
                    SentAmount = transaction.SentAmount,
                    TransactionDate = transaction.TransactionDate,
                    TransactionType = transaction.TransactionType,
                    Message = transaction.Message,
                };

                transactionsVM.Add(vm);

            }



            return View(transactionsVM.AsEnumerable());
        }


    }
}
