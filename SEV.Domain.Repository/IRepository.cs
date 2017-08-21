using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEV.Domain.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> All();
        TEntity GetById(object id);
        IEnumerable<TEntity> GetByIdList(IEnumerable<object> ids);

        Task<IEnumerable<TEntity>> AllAsync();
        Task<TEntity> GetByIdAsync(object id);
        Task<IEnumerable<TEntity>> GetByIdListAsync(IEnumerable<object> ids);

        RepositoryQuery<TEntity> Query();

        TEntity Insert(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
    }
}