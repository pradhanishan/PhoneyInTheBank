using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        IBankAccountRepository BankAccount { get; }

        IApplicationUserRepository ApplicationUser { get; }

        ILoanRepository Loan { get; }

        ITransactionHistoryRepository TransactionHistory { get; }

        IPresentRepository Present { get; }

        IScoreRepository Score { get; }

        IInvestmentRepository Investment { get; }

        IOrganizationRepository Organization { get; }

        Task Save();
    }
}
