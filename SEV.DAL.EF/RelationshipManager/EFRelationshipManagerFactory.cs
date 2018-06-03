using SEV.Domain.Model;
using System;

namespace SEV.DAL.EF
{
    internal class EFRelationshipManagerFactory : IEFRelationshipManagerFactory
    {
        private readonly IDbContext m_context;
        private readonly IReferenceContainer m_refContainer;

        public EFRelationshipManagerFactory(IDbContext context, IReferenceContainer container)
        {
            m_context = context;
            m_refContainer = container;
        }

        public IEFRelationshipManager<TEntity> CreateRelationshipManager<TEntity>(DomainEvent domainEvent)
            where TEntity : Entity
        {
            switch (domainEvent)
            {
                case DomainEvent.Create:
                    return new EFCreateRelationshipManager<TEntity>(m_context, m_refContainer);
                case DomainEvent.Update:
                    return new EFUpdateRelationshipManager<TEntity>(m_context, m_refContainer);
                case DomainEvent.Delete:
                    return new EFDeleteRelationshipManager<TEntity>(m_context, m_refContainer);
                default:
                    throw new ArgumentException("Invalid DomainEvent value.", "domainEvent");
            }
        }
    }
}