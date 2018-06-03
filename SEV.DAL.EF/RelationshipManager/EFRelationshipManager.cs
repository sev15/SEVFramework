using SEV.Domain.Model;
using System.Data.Entity;
using System.Reflection;

namespace SEV.DAL.EF
{
    internal abstract class EFRelationshipManager<TEntity> : IEFRelationshipManager<TEntity> where TEntity : Entity
    {
        private readonly IDbContext m_context;
        private readonly IReferenceContainer m_refContainer;

        protected EFRelationshipManager(IDbContext context, IReferenceContainer container)
        {
            m_context = context;
            m_refContainer = container;
        }

        public abstract void PrepareRelationships(TEntity entity);

        public virtual void RestoreRelationships(TEntity entity)
        {
            m_refContainer.RestoreReferences(entity);
        }

        protected void ArrangeRelationships(TEntity entity, bool attachEntity = false)
        {
            m_refContainer.AnalyzeReferences(entity);

            if (attachEntity)
            {
                EnsureEntityAttached(entity);
            }

            var dbContext = (DbContext)m_context;

            foreach (PropertyInfo propInfo in m_refContainer.GetRelationships())
            {
                ArrangeEntityRelationship(propInfo, entity, dbContext);
            }
            foreach (PropertyInfo propInfo in m_refContainer.GetChildCollections(entity))
            {
                ArrangeChildCollection(propInfo, entity, dbContext);
            }
        }

        private void EnsureEntityAttached(TEntity entity)
        {
            if (m_context.GetEntityState(entity) == EntityState.Detached)
            {
                m_context.Set<TEntity>().Attach(entity);
            }
        }

        protected virtual void ArrangeEntityRelationship(PropertyInfo propInfo, TEntity entity, DbContext dbContext)
        {
            var relatedEntity = propInfo.GetValue(entity);
            if ((relatedEntity != null) && (dbContext.Entry(relatedEntity).State == EntityState.Detached))
            {
                var relatedEntitySet = dbContext.Set(propInfo.PropertyType);
                relatedEntitySet.Attach(relatedEntity);
                dbContext.Entry(relatedEntity).State = EntityState.Unchanged;
            }
        }

        protected abstract void ArrangeChildCollection(PropertyInfo propInfo, TEntity entity, DbContext dbContext);

        protected DbSet GetChildDbSet(DbContext dbContext, object collection)
        {
            var childType = collection.GetType().GenericTypeArguments[0];

            return dbContext.Set(childType);
        }
    }
}