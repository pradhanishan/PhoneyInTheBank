using DataContext.Data;
using Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Update(Investment Entity)
        {
            _db.Update(Entity);
        }
    }
}
