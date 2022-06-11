using DataContext.Data;
using PhoneyInTheBank.Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BankAccountRepository : Repository<BankAccount>, IBankAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public BankAccountRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BankAccount Entity)
        {
            _db.Update(Entity);
        }

        public int GenerateAccountNumber()
        {
            if (_db.BankAccount.Count() == 0)
            {
                return 1000000000;
            }
            return _db.BankAccount.Max(x => x.AccountNumber) + 1;
        }
    }
}
