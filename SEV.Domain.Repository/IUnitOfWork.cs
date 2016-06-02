using SEV.Domain.Model;

namespace SEV.Domain.Repository
{
    public interface IUnitOfWork : System.IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
        IRelationshipManager<TEntity> RelationshipManager<TEntity>() where TEntity : Entity;
        DomainQueryProvider DomainQueryProvider();
        void SaveChanges();
    }
}