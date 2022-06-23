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
    internal class InvestmentRepository : Repository<Investment>, IInvestmentRepository
    {
        private readonly ApplicationDbContext _db;
        public InvestmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Investment> GetUserInvestments(Expression<Func<Investment, bool>> filter)
        {
            return _db.Investment.Where(filter).AsEnumerable();
        }

        public void Update(Investment Entity)
        {
            _db.Update(Entity);
        }
    }
}
