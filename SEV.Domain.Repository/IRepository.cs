using System.Collections.Generic;

namespace SEV.Domain.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> All();
        TEntity GetById(object id);
        IEnumerable<TEntity> GetByIdList(IEnumerable<object> ids);

        RepositoryQuery<TEntity> Query();

        TEntity Insert(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
    }
}