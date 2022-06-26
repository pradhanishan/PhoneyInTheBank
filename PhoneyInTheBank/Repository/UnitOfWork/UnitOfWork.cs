using DataContext.Data;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IBankAccountRepository BankAccount { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public ILoanRepository Loan { get; private set; }

        public ITransactionHistoryRepository TransactionHistory { get; private set; }

        public IPresentRepository Present { get; private set; }

        public IScoreRepository Score { get; private set; }

        public IInvestmentRepository Investment { get; private set; }

        public IOrganizationRepository Organization { get; private set; }


        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            BankAccount = new BankAccountRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Loan = new LoanRepository(_db);
            TransactionHistory = new TransactionHistoryRepository(_db);
            Present = new PresentRepository(_db);
            Score = new ScoreRepository(_db);
            Investment = new InvestmentRepository(_db);
            Organization = new OrganizationRepository(_db);

        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
