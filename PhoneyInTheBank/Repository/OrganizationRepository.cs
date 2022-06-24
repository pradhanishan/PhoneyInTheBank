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
    internal class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        private readonly ApplicationDbContext _db;
        public OrganizationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Organization> GetUninvestedOrganizations(string user)
        {

            var investedOrganizations = _db.Organization.Join(_db.Investment.Where(x => x.ApplicationUser.Email == user && x.ActiveFlag), o => o.Name, i => i.Organization.Name, (o, i) => o);

            List<Organization> uninvestedOrganizations = new List<Organization>();

            foreach (var organization in _db.Organization)
            {
                bool investedFlag = false;

                foreach (var investedOrganization in investedOrganizations)
                {
                    if (organization.Name == investedOrganization.Name)
                    {
                        investedFlag = !investedFlag;

                    }
                }
                if (!investedFlag)
                {
                    uninvestedOrganizations.Add(organization);
                }
            }

            return uninvestedOrganizations.AsEnumerable();
        }

        public async Task Seed()
        {
            var organizations = new List<Organization>()
            {
                new Organization{Name="Meta",},
                new Organization{Name="Apple",},
                new Organization{Name="Amazon",},
                new Organization{Name="Netflix",},
                new Organization{Name="Google",}

            };

            foreach (var organization in organizations)
            {
                if (_db.Organization.FirstOrDefault(x => x.Name == organization.Name) == null)
                {
                    await _db.Organization.AddAsync(organization);
                };
            }


        }
    }
}
