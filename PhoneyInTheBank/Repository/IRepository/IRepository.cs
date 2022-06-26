using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IRepository<T> where T : class
    {

        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter);

        Task<IEnumerable<T>> GetAll();

        Task Add(T Entity);

        void Remove(T Entity);

        void RemoveRange(IEnumerable<T> entity);

    }
}
