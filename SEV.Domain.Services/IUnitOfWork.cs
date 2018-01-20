using SEV.Domain.Model;
using SEV.Domain.Services.Data;
using SEV.Domain.Services.Logic;

namespace SEV.Domain.Services
{
    public interface IUnitOfWork : System.IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
        IRelationshipsStripper<TEntity> RelationshipsStripper<TEntity>() where TEntity : Entity;
        IDomainEventsAggregator DomainEventsAggregator();
        IRelationshipsLoader<TEntity> RelationshipsLoader<TEntity>() where TEntity : Entity;
        IDomainQueryHandler<TResult> CreateDomainQueryHandler<TResult>(string queryName);
        void SaveChanges();
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}