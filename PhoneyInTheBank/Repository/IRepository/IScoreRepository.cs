using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IScoreRepository : IRepository<Score>
    {
        public void IncreaseLuck(Score Entity, int value);
        public void IncreaseGoodwill(Score Entity, int value);

        public void IncreaseTrust(Score Entity, int value);

        public void IncreaseFinancialStatus(Score Entity, int value);

        public void DecreaseLuck(Score Entity, int value);
        public void DecreaseGoodwill(Score Entity, int value);

        public void DecreaseTrust(Score Entity, int value);

        public void DecreaseFinancialStatus(Score Entity, int value);


        public void ToogleBankruptcy(Score Entity, bool bankruptFlag);

        public void Update(Score Entity);
    }
}
