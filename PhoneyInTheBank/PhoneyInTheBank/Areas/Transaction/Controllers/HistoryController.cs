using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.UnitOfWork;
using ViewModels;

namespace PhoneyInTheBank.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int startingNumber, string direction)
        {
            try
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
                ViewData["TotalPages"] = numberOfTransactions % 5 == 0 ? (numberOfTransactions / 5) : (numberOfTransactions / 5) + 1;




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
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }
    }
}
