using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.DAL.EF
{
    public abstract class SEVDbContext : DbContext, IDbContext
    {
        protected SEVDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        protected SEVDbContext()
        {
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class
        {
            return Entry(entity).State;
        }

        public void SetEntityState<TEntity>(TEntity entity, EntityState value) where TEntity : class
        {
            Entry(entity).State = value;
        }

        public void LoadEntityReference<TEntity>(TEntity entity, Expression<Func<TEntity, object>> navigationProperty)
            where TEntity : class
        {
            Entry(entity).Reference(navigationProperty).Load();
        }

        public void LoadEntityCollection<TEntity>(TEntity entity, string navigationProperty) where TEntity : class
        {
            Entry(entity).Collection(navigationProperty).Load();
        }

        public object GetEntityReferenceId<TEntity>(TEntity entity, string navigationProperty) where TEntity : class
        {
            RelationshipManager relManager =
                    ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetRelationshipManager(entity);
            var relationshipFilter = String.Concat("_", navigationProperty);
            IRelatedEnd entityRef = relManager.GetAllRelatedEnds().FirstOrDefault(re =>
                                        (re is EntityReference) && re.RelationshipName.EndsWith(relationshipFilter));
            if (entityRef == null)
            {
                throw new ArgumentException("Invalid argument", "navigationProperty");
            }

            var entityKey = ((EntityReference)entityRef).EntityKey;

            return entityKey == null ? null : entityKey.EntityKeyValues[0].Value;
        }
    }
}