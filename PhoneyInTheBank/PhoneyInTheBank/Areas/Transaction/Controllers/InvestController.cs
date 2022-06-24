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

            var user = User.Identity?.Name;
            ApplicationUser applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.UserName == user);
            if (user == null) return NotFound();

            // Auto create organizations that don't exist in deatabase
            await _unitOfWork.Organization.Seed();
            await _unitOfWork.Save();


            List<InvestVM> investmentsVM = new List<InvestVM>();

            IEnumerable<Investment> investments = _unitOfWork.Investment.GetUserInvestments(x => x.ApplicationUser == applicationUser);

            foreach (var investment in investments)
            {
                InvestVM iv = new()
                {

                    OrganizationName = investment.Organization.Name,
                    InvestedFlag = true,
                    LastCollectedDate = investment.LastCollectedDate,
                    Loss = investment.Loss,
                    Profit = investment.Profit,
                    InvestmentAmount = investment.InvestmentAmount,
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

        public async Task<IActionResult> InvestInOrganization(string organization)
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

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> InvestInOrganization(InvestInOrganizationVM investment)
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

            var user = User.Identity.Name;

            ApplicationUser applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Email == user);
            if (applicationUser == null) return NotFound();

            Organization organization = await _unitOfWork.Organization.GetFirstOrDefault(x => x.Name == investment.Organization);

            Investment newInvestment = new()
            {
                ApplicationUser = applicationUser,
                Organization = organization,
                InvestmentAmount = investment.InvestmentAmount,
            };



            await _unitOfWork.Investment.Add(newInvestment);
            await _unitOfWork.Save();

            TempData["Success"] = "Investment completed successfully";


            return RedirectToAction("Index", "Invest", new { Area = "Transaction" });

        }

    }
}
