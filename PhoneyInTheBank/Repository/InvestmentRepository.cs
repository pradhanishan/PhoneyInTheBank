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

        public Task CancelInvestment(string user, string organization)
        {
            Investment investment = _db.Investment.FirstOrDefault(x => x.ApplicationUser.Email == user && x.Organization.Name == organization);
            investment.Profit = investment.Loss = investment.InvestmentAmount = 0;
            investment.DaysInvested = 0;
            investment.ActiveFlag = false;



            return Task.CompletedTask;

        }


        public IEnumerable<Investment> GetUserInvestments(Expression<Func<Investment, bool>> filter)
        {
            return _db.Investment.Where(filter).AsEnumerable();
        }

        public void Update(Investment Entity)
        {
            Entity.LastCollectedDate = DateTimeOffset.UtcNow;
            Entity.UpdatedDate = DateTimeOffset.UtcNow;
            _db.Update(Entity);
        }
    }
}
