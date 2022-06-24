using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IInvestmentRepository : IRepository<Investment>
    {
        public void Update(Investment Entity);

        public IEnumerable<Investment> GetUserInvestments(Expression<Func<Investment, bool>> filter);

        public Task CancelInvestment(string user, string organization);

    }
}
