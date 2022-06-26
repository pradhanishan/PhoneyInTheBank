using PhoneyInTheBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IBankAccountRepository : IRepository<BankAccount>
    {
        public void Update(BankAccount Entity);
        public int GenerateAccountNumber();

        public Task<BankAccount> GetUserBankAccount(string user);

    }
}
