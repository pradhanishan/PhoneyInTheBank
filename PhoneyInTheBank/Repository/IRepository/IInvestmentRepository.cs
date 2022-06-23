using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IInvestmentRepository : IRepository<Investment>
    {
        public void Update(Investment Entity);
    }
}
