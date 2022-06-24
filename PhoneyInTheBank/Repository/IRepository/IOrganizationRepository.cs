using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOrganizationRepository : IRepository<Organization>
    {

        public IEnumerable<Organization> GetUninvestedOrganizations(string user);
        public Task Seed();

    }
}
