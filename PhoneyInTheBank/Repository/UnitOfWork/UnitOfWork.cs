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

        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            BankAccount = new BankAccountRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Loan = new LoanRepository(_db);

        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
