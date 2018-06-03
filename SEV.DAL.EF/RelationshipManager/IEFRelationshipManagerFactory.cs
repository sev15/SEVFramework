using SEV.Domain.Model;

namespace SEV.DAL.EF
{
    public interface IEFRelationshipManagerFactory
    {
        IEFRelationshipManager<TEntity> CreateRelationshipManager<TEntity>(DomainEvent domainEvent) where TEntity : Entity;
    }
}