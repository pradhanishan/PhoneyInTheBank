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
        void Save();
    }
}
