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
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        private readonly ApplicationDbContext _db;
        public LoanRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Loan Entity)
        {
            _db.Update(Entity);
        }
    }
}
