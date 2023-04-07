using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mongo
{
    public interface IRepository<TKey, TValue> where TValue : IEntity<TKey>
    {
        Task Create(TValue entity);
        Task<IReadOnlyCollection<TValue>> GetAll();
        Task<IReadOnlyCollection<TValue>> GetAll(Expression<Func<TValue,bool>> filter);
        // TODO add nullability
        Task<TValue> Get(TKey id);
        Task<TValue> Get(Expression<Func<TValue, bool>> filter);
        Task Remove(TKey id);
        Task Update(TValue enitity);
    }
}
