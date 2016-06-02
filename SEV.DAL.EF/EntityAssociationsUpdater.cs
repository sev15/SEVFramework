using SEV.Common;
using SEV.Domain.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SEV.DAL.EF
{
    public abstract class EntityAssociationsUpdater
    {
        public abstract void UpdateAssociations(Entity entity, IDbContext context);

        protected void UpdateReference<TEntity>(TEntity entity, DbContext context,
            Expression<Func<TEntity, object>> navigationPropertySelector)
            where TEntity : Entity
        {
            PropertyInfo navigationPropertyInfo = LambdaExpressionHelper.GetExpressionMethod(navigationPropertySelector);

            var relatedEntity = (Entity)navigationPropertyInfo.GetValue(entity);

            var stateManager = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager;
            var parentRelationship = stateManager.GetRelationshipManager(entity).GetAllRelatedEnds()
                                                 .OfType<EntityReference>()
                                                 .First(re => re.RelationshipName.EndsWith("Parent"));
            // TODO : verify how we could check if a relationship was loaded, especially when this relationship is removed.
            if (relatedEntity != null)
            {
                stateManager.ChangeRelationshipState(entity, relatedEntity, navigationPropertySelector, EntityState.Detached);
                navigationPropertyInfo.SetValue(entity, null);
            }
            context.Entry(entity).Reference(navigationPropertySelector).Load();
            int? parentRelationshipId = parentRelationship.EntityKey == null ? (int?)null
                                                        : (int)parentRelationship.EntityKey.EntityKeyValues[0].Value;

            if (relatedEntity != null)
            {
                if (!parentRelationshipId.HasValue)
                {
                    stateManager.ChangeRelationshipState(entity, relatedEntity, navigationPropertySelector, EntityState.Added);
                    return;
                }
                if (!relatedEntity.Id.Equals(parentRelationshipId.Value))
                {
                    var targetEntity = (Entity)navigationPropertyInfo.GetValue(entity);
                    stateManager.ChangeRelationshipState(entity, targetEntity, navigationPropertySelector, EntityState.Deleted);
                    stateManager.ChangeRelationshipState(entity, relatedEntity, navigationPropertySelector, EntityState.Added);
                }
            }
            else if (parentRelationshipId.HasValue)
            {
                var targetEntity = (Entity)navigationPropertyInfo.GetValue(entity);
                stateManager.ChangeRelationshipState(entity, targetEntity, navigationPropertySelector, EntityState.Deleted);
            }
        }
    }
}