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
    public class LoanController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var username = User.Identity?.Name;
                ApplicationUser user = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity.Name);

                IEnumerable<LoanVM> loanTypes = new List<LoanVM>()
            {
                new LoanVM {
                    Id=1,
                    LoanType=CLoanType.SmallLoan,
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=CLoanType.SmallLoanInterestRate,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/4
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/4)},
                new LoanVM {
                    Id=2,
                    LoanType=CLoanType.AveragelLoan,
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=CLoanType.AverageLoanInterestRate,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/2
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/2)},
                new LoanVM {
                    Id=3,
                    LoanType=CLoanType.LargeLoan,
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=CLoanType.LargeLoanInterestRate,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount
                    , TotalAmount=3 * bankAccount.OperativeAmount},
            };

                Loan existingLoan = await _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);


                if (existingLoan != null)
                {
                    return RedirectToAction("PayLoan", "Loan", new { area = "Transaction" });
                }

                return View(loanTypes);
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        // Apply for loan if you do not have an existing loan
        public async Task<IActionResult> ApplyLoan(int? id)
        {
            try
            {
                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity.Name);

                Score score = await _unitOfWork.Score.GetFirstOrDefault(x => x.ApplicationUser.Email == User.Identity.Name);


                IEnumerable<LoanVM> loanTypes = new List<LoanVM>()
            {
                new LoanVM {
                    Id=1,
                    LoanType=CLoanType.SmallLoan,
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=CLoanType.SmallLoanInterestRate,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/4
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/4)},
                new LoanVM {
                    Id=2,
                    LoanType=CLoanType.AveragelLoan,
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=CLoanType.AverageLoanInterestRate,
                    LoanAmount = bankAccount.OperativeAmount + bankAccount.OperativeAmount/2
                    , TotalAmount=bankAccount.OperativeAmount + (2*bankAccount.OperativeAmount/2)},
                new LoanVM {
                    Id=3,
                    LoanType=CLoanType.LargeLoan,
                    OperativeAmount=bankAccount.OperativeAmount,
                    InterestRate=CLoanType.LargeLoanInterestRate,
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

                if ((loan.LoanAmount / bankAccount.OperativeAmount) * 100 >= 5 && (loan.LoanAmount / bankAccount.OperativeAmount) * 100 < 5)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 1);
                }
                if ((loan.LoanAmount / bankAccount.OperativeAmount) * 100 >= 10 && (loan.LoanAmount / bankAccount.OperativeAmount) * 100 < 50)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 3);
                }
                if ((loan.LoanAmount / bankAccount.OperativeAmount) * 100 >= 50)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 10);
                }

                Loan existingLoan = await _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);

                if (existingLoan != null)
                {
                    return RedirectToAction("Loan", "Loan", new { area = "Transaction" });
                }

                bankAccount.OperativeAmount += loan.LoanAmount;
                bankAccount.LoanAmount = loan.LoanAmountWithInterest;

                await _unitOfWork.Loan.Add(loan);
                _unitOfWork.BankAccount.Update(bankAccount);

                TransactionHistory trx = new()
                {
                    ReceivedAmount = selectedLoanType.LoanAmount,
                    TransactionType = "LT",
                    SentAmount = 0,
                    User = User.Identity.Name,
                    Message = CTransaction.GetLoanTakenLog(selectedLoanType.LoanType, selectedLoanType.LoanAmount),
                };
                await _unitOfWork.TransactionHistory.Add(trx);
                await _unitOfWork.Save();

                TempData["Success"] = "Loan processing completed.";

                return RedirectToAction("Index", "User", new { area = "User" });
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        public async Task<IActionResult> PayLoan()
        {
            try
            {
                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity.Name); Loan existingLoan = await _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);

                LoanPaymentVM loanPayment = new()
                {
                    AmountTaken = existingLoan.LoanAmount,
                    InterestRate = existingLoan.InterestRate,
                    AmountWithInterest = existingLoan.LoanAmountWithInterest,
                    AmountLeftToPay = existingLoan.LeftToPayWithInterest,
                    TakenOn = existingLoan.CreatedDate,
                    OperativeAmount = bankAccount.OperativeAmount,
                };

                return View(loanPayment);
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> PayLoan(LoanPaymentVM loanPayment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(loanPayment);
                }

                var username = User.Identity?.Name;
                ApplicationUser user = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == username);

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == user);


                Score score = await _unitOfWork.Score.GetFirstOrDefault(x => x.ApplicationUser == user);

                Loan existingLoan = await _unitOfWork.Loan.GetFirstOrDefault(x => x.BankAccount.Equals(bankAccount) && x.ActiveFlag);

                if (loanPayment.ClearanceAmount > existingLoan.LeftToPayWithInterest)
                {

                    ModelState.AddModelError(string.Empty, CModelError.PayingMoreThanOweError);
                    return View(loanPayment);


                }
                if (bankAccount.OperativeAmount < loanPayment.ClearanceAmount)
                {
                    ModelState.AddModelError(string.Empty, CModelError.InsufficientFundError);
                    return View(loanPayment);
                }

                if ((loanPayment.ClearanceAmount / bankAccount.OperativeAmount) * 100 > 90)
                {
                    ModelState.AddModelError(string.Empty, CModelError.NinetyPercentLoanPaymentThresholdError);
                    return View(loanPayment);
                }

                bankAccount.OperativeAmount -= loanPayment.ClearanceAmount;
                bankAccount.LoanAmount -= loanPayment.ClearanceAmount;
                existingLoan.LeftToPayWithInterest -= loanPayment.ClearanceAmount;
                loanPayment.AmountLeftToPay -= loanPayment.ClearanceAmount;

                if ((loanPayment.ClearanceAmount / bankAccount.OperativeAmount) * 100 >= 5 && (loanPayment.ClearanceAmount / bankAccount.OperativeAmount) * 100 < 5)
                {
                    _unitOfWork.Score.DecreaseFinancialStatus(score, 1);
                }
                if ((loanPayment.ClearanceAmount / bankAccount.OperativeAmount) * 100 >= 10 && (loanPayment.ClearanceAmount / bankAccount.OperativeAmount) * 100 < 50)
                {
                    _unitOfWork.Score.DecreaseFinancialStatus(score, 3);
                }
                if ((loanPayment.ClearanceAmount / bankAccount.OperativeAmount) * 100 >= 50)
                {
                    _unitOfWork.Score.DecreaseFinancialStatus(score, 10);
                }

                if (existingLoan.LeftToPayWithInterest == 0)
                {
                    _unitOfWork.Score.IncreaseTrust(score, 10);
                    existingLoan.ActiveFlag = false;
                }
                existingLoan.UpdatedDate = DateTimeOffset.Now;

                TransactionHistory trx = new()
                {
                    SentAmount = loanPayment.ClearanceAmount,
                    TransactionType = CTransaction.TRXTypeLoanGiven,
                    ReceivedAmount = 0,
                    User = User.Identity.Name,
                    Message = CTransaction.GetLoanPaidLog(loanPayment.ClearanceAmount, loanPayment.AmountWithInterest),
                };

                _unitOfWork.BankAccount.Update(bankAccount);
                _unitOfWork.Loan.Update(existingLoan);
                await _unitOfWork.TransactionHistory.Add(trx);
                await _unitOfWork.Save();

                TempData["Success"] = CTransaction.PaymentCompleted;

                return RedirectToAction("Index", "User", new { Area = "User" });
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }

        }

    }
}
