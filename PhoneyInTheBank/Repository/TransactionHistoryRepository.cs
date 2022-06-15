using DataContext.Data;
using Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class TransactionHistoryRepository : Repository<TransactionHistory>, ITransactionHistoryRepository
    {
        private readonly ApplicationDbContext _db;
        public TransactionHistoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<TransactionHistory> GetUserTransactions(Expression<Func<TransactionHistory, bool>> filter)
        {
            return _db.TransactionHistory.Where(filter).AsEnumerable();
        }


    }
}
