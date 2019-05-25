using System.Collections;
using System.Collections.Generic;
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

        public virtual void PrepareRelationships(TEntity entity)
        {
            ArrangeRelationships(entity);
        }

        public virtual void RestoreRelationships(TEntity entity)
        {
            m_refContainer.RestoreReferences(entity);
        }

        protected bool AttachEntity { get; set; }

        private void ArrangeRelationships(TEntity entity)
        {
            m_refContainer.AnalyzeReferences(entity);

            if (AttachEntity)
            {
                EnsureEntityAttached(entity);
            }

            var dbContext = (DbContext)m_context;

            foreach (PropertyInfo propInfo in m_refContainer.GetRelationships())
            {
                ArrangeEntityRelationship(propInfo, entity, dbContext);
            }
            foreach (var collectionInfo in m_refContainer.GetChildCollections(entity))
            {
                ArrangeChildCollection(collectionInfo, entity, dbContext);
            }
        }

        private void EnsureEntityAttached(TEntity entity)
        {
            if (m_context.GetEntityState(entity) == EntityState.Detached)
            {
                m_context.Set<TEntity>().Attach(entity);
            }
        }

        protected virtual void ArrangeEntityRelationship(PropertyInfo propInfo, Entity entity, DbContext dbContext)
        {
            var relatedEntity = propInfo.GetValue(entity);
            if (relatedEntity == null)
            {
                return;
            }
            if (dbContext.Entry(relatedEntity).State == EntityState.Detached)
            {
                dbContext.Set(propInfo.PropertyType).Attach(relatedEntity);
            }
            dbContext.Entry(relatedEntity).State = EntityState.Unchanged;
        }

        protected abstract void ArrangeChildCollection(KeyValuePair<PropertyInfo, ICollection> collectionInfo,
                                                       TEntity entity, DbContext dbContext);

        protected DbSet GetChildDbSet(DbContext dbContext, object collection)
        {
            var childType = collection.GetType().GenericTypeArguments[0];

            return dbContext.Set(childType);
        }

        protected void ArrangeChildRelationships(Entity child)
        {
            var dbContext = (DbContext)m_context;
            foreach (PropertyInfo propInfo in m_refContainer.GetChildRelationships())
            {
                ArrangeChildRelationship(propInfo, child, dbContext);
            }
        }

        protected virtual void ArrangeChildRelationship(PropertyInfo propInfo, Entity childEntity, DbContext dbContext)
        {
            ArrangeEntityRelationship(propInfo, childEntity, dbContext);
        }
    }
}