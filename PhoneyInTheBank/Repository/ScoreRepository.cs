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
    public class ScoreRepository : Repository<Score>, IScoreRepository
    {
        private readonly ApplicationDbContext _db;
        public ScoreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void DecreaseFinancialStatus(Score Entity, int value)
        {
            if (Entity.FinancialScore - value >= 0)
            {
                Entity.FinancialScore -= value;
                Update(Entity);
            }
        }

        public void DecreaseGoodwill(Score Entity, int value)
        {
            if (Entity.GoodWillScore - value >= 0)
            {
                Entity.GoodWillScore -= value;
                Update(Entity);
            }
        }

        public void DecreaseLuck(Score Entity, int value)
        {
            if (Entity.LuckScore - value >= 0)
            {
                Entity.LuckScore -= value;
                Update(Entity);
            }
        }

        public void DecreaseTrust(Score Entity, int value)
        {
            if (Entity.TrustScore - value >= 0)
            {
                Entity.TrustScore -= value;
                Update(Entity);
            }
        }

        public void IncreaseFinancialStatus(Score Entity, int value)
        {
            if (Entity.FinancialScore + value <= 100)
            {
                Entity.FinancialScore += value;
                Update(Entity);
            }
        }

        public void IncreaseGoodwill(Score Entity, int value)
        {
            if (Entity.GoodWillScore + value <= 100)
            {
                Entity.GoodWillScore += value;
                Update(Entity);
            }
        }

        public void IncreaseLuck(Score Entity, int value)
        {
            if (Entity.LuckScore + value <= 100)
            {
                Entity.LuckScore += value;
                Update(Entity);
            }
        }

        public void IncreaseTrust(Score Entity, int value)
        {
            if (Entity.TrustScore + value <= 100)
            {
                Entity.TrustScore += value;
                Update(Entity);
            }
        }

        public void ToogleBankruptcy(Score Entity, bool bankruptFlag)
        {
            Entity.Bankrupt = bankruptFlag;
            Update(Entity);
        }

        public void Update(Score Entity)
        {
            Entity.UpdatedDate = DateTimeOffset.UtcNow;
            _db.Update(Entity);
        }
    }
}
