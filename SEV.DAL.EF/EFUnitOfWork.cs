using System;
using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Repository;
using SEV.Domain.Model;

namespace SEV.DAL.EF
{
    internal class EFUnitOfWork : IUnitOfWork
    {
        private readonly IDbContext m_context;

        public EFUnitOfWork(IDbContext context)
        {
            m_context = context;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            return new EFRepository<TEntity>(m_context);
        }

        public IRelationshipManager<TEntity> RelationshipManager<TEntity>() where TEntity : Entity
        {
            return new EFRelationshipManager<TEntity>(m_context);
        }

        public DomainQueryProvider DomainQueryProvider()
        {
            var factory = ServiceLocator.Current.GetInstance<IDomainQueryHandlerFactory>();

            return new DomainQueryProvider(factory);
        }

        public void SaveChanges()
        {
            if (m_context != null)
            {
                m_context.SaveChanges();
            }
        }

        private bool m_disposed;

        public void Dispose()
        {
            if (!m_disposed)
            {
                if (m_context != null)
                {
                    m_context.Dispose();
                }
                m_disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        //void BeginTransaction();
        //void Rollback();
    }
}