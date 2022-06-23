using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOrganizationRepository : IRepository<Organization>
    {


        public Task Seed();

    }
}
