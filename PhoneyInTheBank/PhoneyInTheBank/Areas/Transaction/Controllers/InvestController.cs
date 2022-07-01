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
    public class InvestController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvestController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = User.Identity?.Name;
                ApplicationUser applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.UserName == user);
                if (user == null) return NotFound();

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == applicationUser);
                if (bankAccount == null) return NotFound();

                if(bankAccount.OperativeAmount ==0 && bankAccount.InvestmentAmount == 0)
                {
                    return RedirectToAction("Index", "User", new { Area = "User" });
                }


                // Auto create organizations that don't exist in deatabase
                await _unitOfWork.Organization.Seed();



                await _unitOfWork.Save();


                List<InvestVM> investmentsVM = new List<InvestVM>();

                IEnumerable<Investment> investments = await GenerateROI();

                // Generate Profit or Loss for each investment




                await _unitOfWork.Save();

                foreach (var investment in investments)
                {
                    InvestVM iv = new()
                    {

                        OrganizationName = investment.Organization.Name,
                        InvestedFlag = true,
                        LastCollectedDate = investment.LastCollectedDate,
                        Loss = investment.Loss,
                        Profit = investment.Profit,
                        Net = investment.Profit - investment.Loss,
                        InvestmentAmount = investment.InvestmentAmount,
                        DaysInvested = investment.DaysInvested,
                    };

                    investmentsVM.Add(iv);
                }

                IEnumerable<Organization> uninvestedOrganizations = _unitOfWork.Organization.GetUninvestedOrganizations(user);



                foreach (var organization in uninvestedOrganizations)
                {
                    InvestVM iv = new()
                    {
                        OrganizationName = organization.Name,
                        InvestedFlag = false,
                    };
                    investmentsVM.Add(iv);
                }

                return View(investmentsVM.AsEnumerable());
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        public async Task<IActionResult> InvestInOrganization(string organization)
        {
            try
            {
                if (organization == null) return NotFound();

                var user = User.Identity.Name;

                ApplicationUser applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == user);
                if (applicationUser == null) return NotFound();

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == applicationUser);
                if (bankAccount == null) return NotFound();

                InvestInOrganizationVM investInOrganizationVM = new InvestInOrganizationVM()
                {
                    Organization = organization,
                    InvestmentBalance = bankAccount.InvestmentAmount,
                };



                return View(investInOrganizationVM);
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> InvestInOrganization(InvestInOrganizationVM investment)
        {
            try
            {
                InvestInOrganizationVM investInOrganizationVM = new()
                {
                    Organization = investment.Organization,
                    InvestmentBalance = investment.InvestmentBalance,
                    InvestmentAmount = investment.InvestmentAmount
                };

                if (!ModelState.IsValid)
                {
                    return View(investInOrganizationVM);
                }

                if (investment.InvestmentAmount < 1 || investment.InvestmentAmount > investment.InvestmentBalance)
                {
                    ModelState.AddModelError(string.Empty, "Investment amount must be between 0 and " + investment.InvestmentBalance);
                    return View(investInOrganizationVM);
                }

                var user = User.Identity?.Name;

                ApplicationUser applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == user);
                if (applicationUser == null) return NotFound();

                BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser == applicationUser);
                if (bankAccount == null) return NotFound();

                Organization organization = await _unitOfWork.Organization.GetFirstOrDefault(x => x.Name == investment.Organization);


                Investment existingInvestment = await _unitOfWork.Investment.GetFirstOrDefault(x => x.ApplicationUser.Email == user && x.Organization.Name == organization.Name);

                if (existingInvestment != null)
                {
                    existingInvestment.ActiveFlag = true;
                    existingInvestment.InvestmentAmount = investment.InvestmentAmount;
                    bankAccount.InvestmentAmount -= investment.InvestmentAmount;

                    _unitOfWork.Investment.Update(existingInvestment);
                    _unitOfWork.BankAccount.Update(bankAccount);

                }

                if (existingInvestment == null)
                {
                    Investment newInvestment = new()
                    {
                        ApplicationUser = applicationUser,
                        Organization = organization,
                        InvestmentAmount = investment.InvestmentAmount,
                    };

                    bankAccount.InvestmentAmount -= investment.InvestmentAmount;
                    await _unitOfWork.Investment.Add(newInvestment);
                    _unitOfWork.BankAccount.Update(bankAccount);
                }

                TransactionHistory trx = new()
                {
                    User = user,
                    SentAmount = investment.InvestmentAmount,
                    Message = "invested in " + organization.Name,
                };

                await _unitOfWork.TransactionHistory.Add(trx);
                await _unitOfWork.Save();

                TempData["Success"] = "Investment completed successfully";


                return RedirectToAction("Index", "Invest", new { Area = "Transaction" });
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        public async Task<IActionResult> CollectROI(string organization)
        {
            try
            {

                Investment investment = await _unitOfWork.Investment.GetFirstOrDefault(x => x.ApplicationUser.Email == User.Identity.Name && x.Organization.Name == organization && x.ActiveFlag);


                BankAccount bankAccount = await _unitOfWork.BankAccount.GetUserBankAccount(User.Identity.Name);

                if (bankAccount.InvestmentAmount + investment.Profit - investment.Loss < 0) bankAccount.InvestmentAmount = 0;

                if (bankAccount.InvestmentAmount + investment.Profit - investment.Loss >= 0)
                    bankAccount.InvestmentAmount = bankAccount.InvestmentAmount + investment.Profit - investment.Loss;


                Score score = await _unitOfWork.Score.GetFirstOrDefault(x => x.ApplicationUser.Email == User.Identity.Name);

                if ((bankAccount.InvestmentAmount / bankAccount.OperativeAmount) * 100 >= 5 && (bankAccount.InvestmentAmount / bankAccount.OperativeAmount) * 100 < 5)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 1);
                }
                if ((bankAccount.InvestmentAmount / bankAccount.OperativeAmount) * 100 >= 10 && (bankAccount.InvestmentAmount / bankAccount.OperativeAmount) * 100 < 50)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 3);
                }
                if ((bankAccount.InvestmentAmount / bankAccount.OperativeAmount) * 100 >= 50)
                {
                    _unitOfWork.Score.IncreaseFinancialStatus(score, 10);
                }

                bankAccount.OperativeAmount += bankAccount.InvestmentAmount;


                _unitOfWork.Score.Update(score);
                await _unitOfWork.Investment.CancelInvestment(User.Identity.Name, organization);

                TransactionHistory trx = new()
                {
                    Message = "Received return of investment of " + bankAccount.InvestmentAmount.ToString() + " from " + organization,
                    ReceivedAmount = bankAccount.InvestmentAmount,
                    User = User.Identity.Name,
                };


                bankAccount.InvestmentAmount = 0;
                _unitOfWork.BankAccount.Update(bankAccount);
                await _unitOfWork.TransactionHistory.Add(trx);
                await _unitOfWork.Save();

                TempData["Success"] = "Investment refunded successfully";

                return RedirectToAction("Index", "Invest", new { Area = "Transaction" });
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { Area = "Home" });
            }
        }

        public async Task<IEnumerable<Investment>> GenerateROI()
        {

            BankAccount bankAccount = await _unitOfWork.BankAccount.GetFirstOrDefault(x => x.ApplicationUser.Email == User.Identity.Name);

            IEnumerable<Investment> investments = _unitOfWork.Investment.GetUserInvestments(x => x.ApplicationUser.Email == User.Identity.Name && x.ActiveFlag);

            foreach (var investment in investments)
            {
                TimeSpan ROISpan = DateTimeOffset.UtcNow - investment.LastCollectedDate;

                if (ROISpan.Days > 0) TempData["RevenueGenerated"] = "Y";

                for (int i = 0; i < ROISpan.Days; i++)
                {
                    Random r = new();
                    int profitOrLoss = r.Next(0, 100);
                    if (profitOrLoss < 50)
                    {
                        // Loss
                        float lossAmount = (r.Next(1, 101) * investment.InvestmentAmount) / 100;
                        investment.Loss += lossAmount;
                        if (bankAccount.InvestmentAmount - investment.Loss < 0) bankAccount.InvestmentAmount = 0;
                        else bankAccount.InvestmentAmount -= lossAmount;



                    }
                    if (profitOrLoss >= 50)
                    {
                        // Profit
                        float profitAmount = (r.Next(1, 101) * investment.InvestmentAmount) / 100;
                        investment.Profit += profitAmount;
                        bankAccount.InvestmentAmount += profitAmount;

                    }

                }

                investment.DaysInvested += ROISpan.Days;
                _unitOfWork.Investment.Update(investment);
                _unitOfWork.BankAccount.Update(bankAccount);

            }
            await _unitOfWork.Save();



            return investments;
        }

    }
}
