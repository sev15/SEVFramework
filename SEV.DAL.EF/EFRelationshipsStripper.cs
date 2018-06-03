using SEV.Domain.Model;
using SEV.Domain.Services.Data;

namespace SEV.DAL.EF
{
    public class EFRelationshipsStripper<TEntity> : IRelationshipsStripper<TEntity> where TEntity : Entity
    {
        private readonly IEFRelationshipManagerFactory m_factory;
        private IEFRelationshipManager<TEntity> m_manager;

        public EFRelationshipsStripper(IEFRelationshipManagerFactory factory)
        {
            m_factory = factory;
        }

        public virtual void Strip(TEntity entity, DomainEvent domainEvent)
        {
            m_manager = m_factory.CreateRelationshipManager<TEntity>(domainEvent);
            m_manager.PrepareRelationships(entity);
        }

        public virtual void UnStrip(TEntity entity)
        {
            m_manager.RestoreRelationships(entity);
        }
    }
}
