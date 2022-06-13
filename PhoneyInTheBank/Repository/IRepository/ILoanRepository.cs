using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ILoanRepository : IRepository<Loan>
    {
        public void Update(Loan Entity);

    }
}
