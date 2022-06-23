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

            IEnumerable<Investment> investments = _unitOfWork.Investment.GetUserInvestments(x => x.ApplicationUser == applicationUser);

            IEnumerable<Organization> organizations = await _unitOfWork.Organization.GetAll();

            List<InvestVM> investmentsVM = new List<InvestVM>();

            foreach (var organization in organizations)
            {
                var investVM = new InvestVM() { OrganizationName = organization.Name };
                investmentsVM.Add(investVM);
            }

            if (!investments.Any())
            {
                ViewData["NotInvested"] = "Y";
            }

            if (investments.Any())
            {
                // [TODO] - Important : [Fetch stock range statistics]
            }

            return View(investmentsVM.AsEnumerable());
        }

    }
}
