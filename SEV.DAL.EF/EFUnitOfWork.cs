using System;
using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Model;
using SEV.Domain.Services;
using SEV.Domain.Services.Data;
using SEV.Domain.Services.Logic;

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

        public IRelationshipsStripper<TEntity> RelationshipsStripper<TEntity>() where TEntity : Entity
        {
            return new EFRelationshipsStripper<TEntity>(new EFRelatedEntitiesStateAdjuster(m_context));
        }

        public IDomainEventsAggregator DomainEventsAggregator()
        {
            return new DomainEventsAggregator();
        }

        public IRelationshipsLoader<TEntity> RelationshipsLoader<TEntity>() where TEntity : Entity
        {
            return new EFRelationshipsLoader<TEntity>(m_context);
        }

        public IDomainQueryHandler<TResult> CreateDomainQueryHandler<TResult>(string queryName)
        {
            var handler = ServiceLocator.Current.GetInstance<IDomainQueryHandler>(queryName);
            if (handler is IDomainQueryHandler<TResult>)
            {
                return (IDomainQueryHandler<TResult>)handler;
            }

            throw new InvalidOperationException(string.Format("Invalid result type for '{0}' query", queryName));
        }

        public void SaveChanges()
        {
            if (m_context != null)
            {
                m_context.SaveChanges();
            }
        }

        public async System.Threading.Tasks.Task SaveChangesAsync()
        {
            if (m_context != null)
            {
                await m_context.SaveChangesAsync();
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