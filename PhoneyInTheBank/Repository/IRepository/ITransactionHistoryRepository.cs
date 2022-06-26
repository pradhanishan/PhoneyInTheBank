using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Repository.IRepository
{
    public interface ITransactionHistoryRepository : IRepository<TransactionHistory>
    {

        public IEnumerable<TransactionHistory> GetUserTransactions(Expression<Func<TransactionHistory, bool>> filter);

    }
}
