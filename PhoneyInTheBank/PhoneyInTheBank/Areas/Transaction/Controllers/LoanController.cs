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
    public class LoanController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
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
                return RedirectToAction("PayLoan", "Loan", new { area = "Transaction" });
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
                return RedirectToAction("Loan", "Loan", new { area = "Transaction" });
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

            TempData["Success"] = "Loan processing completed.";

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

            TempData["Success"] = "Payment Completed.";

            return RedirectToAction("Index", "User", new { Area = "User" });
        }

    }
}
