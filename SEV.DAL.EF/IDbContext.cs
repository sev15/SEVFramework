using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace SEV.DAL.EF
{
    public interface IDbContext : IDisposable
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class;
        void SetEntityState<TEntity>(TEntity entity, EntityState value) where TEntity : class;
        void LoadEntityReference<TEntity>(TEntity entity, Expression<Func<TEntity, object>> navigationProperty)
            where TEntity : class;
        void LoadEntityCollection<TEntity>(TEntity entity, string navigationProperty) where TEntity : class;
        object GetEntityReferenceId<TEntity>(TEntity entity, string navigationProperty) where TEntity : class;
        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
    }
}