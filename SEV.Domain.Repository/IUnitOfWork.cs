using SEV.Domain.Model;

namespace SEV.Domain.Repository
{
    public interface IUnitOfWork : System.IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
        IRelationshipManager<TEntity> RelationshipManager<TEntity>() where TEntity : Entity;
        IDomainQueryHandler<TResult> CreateDomainQueryHandler<TResult>(string queryName);
        void SaveChanges();
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}