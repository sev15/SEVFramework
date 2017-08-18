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

        public IDomainQueryHandler<TResult> CreateDomainQueryHandler<TResult>(string queryName)
        {
            var handler = ServiceLocator.Current.GetInstance<IDomainQueryHandler>(queryName);
            if (handler is IDomainQueryHandler<TResult>)
            {
                return (IDomainQueryHandler<TResult>)handler;
            }

            throw new InvalidOperationException($"Invalid result type for '{queryName}' query");
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