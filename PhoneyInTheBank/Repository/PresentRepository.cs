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

    internal class PresentRepository : Repository<Present>, IPresentRepository
    {
        private readonly ApplicationDbContext _db;
        public PresentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Present Entity)
        {

            _db.Update(Entity);
        }
    }
}
