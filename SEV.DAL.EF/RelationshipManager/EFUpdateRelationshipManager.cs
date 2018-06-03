using SEV.Domain.Model;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace SEV.DAL.EF
{
    internal class EFUpdateRelationshipManager<TEntity> : EFRelationshipManager<TEntity> where TEntity : Entity
    {
        public EFUpdateRelationshipManager(IDbContext context, IReferenceContainer container)
            : base(context, container)
        {
        }

        public override void PrepareRelationships(TEntity entity)
        {
            ArrangeRelationships(entity, true);
        }

        protected override void ArrangeEntityRelationship(PropertyInfo propInfo, TEntity entity, DbContext dbContext)
        {
            string propName = propInfo.Name;
            var relatedEntity = (Entity)propInfo.GetValue(entity);

            var stateManager = ((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager;
            var parentRelationship = stateManager.GetRelationshipManager(entity).GetAllRelatedEnds()
                                                 .OfType<EntityReference>()
                                                 .First(re => re.RelationshipName.Contains(propName));
            // TODO : verify how we could check if a relationship was loaded, especially when this relationship is removed.
            if (relatedEntity != null)
            {
                stateManager.ChangeRelationshipState(entity, relatedEntity, propName, EntityState.Detached);
                propInfo.SetValue(entity, null);
            }
            dbContext.Entry(entity).Reference(propName).Load();
            int? parentRelationshipId = parentRelationship.EntityKey == null ? (int?)null
                                                        : (int)parentRelationship.EntityKey.EntityKeyValues[0].Value;

            if (relatedEntity != null)
            {
                if (!parentRelationshipId.HasValue)
                {
                    stateManager.ChangeRelationshipState(entity, relatedEntity, propName, EntityState.Added);
                    return;
                }
                if (!relatedEntity.Id.Equals(parentRelationshipId.Value))
                {
                    var targetEntity = (Entity)propInfo.GetValue(entity);
                    stateManager.ChangeRelationshipState(entity, targetEntity, propName, EntityState.Deleted);
                    stateManager.ChangeRelationshipState(entity, relatedEntity, propName, EntityState.Added);
                }
            }
            else if (parentRelationshipId.HasValue)
            {
                var targetEntity = (Entity)propInfo.GetValue(entity);
                stateManager.ChangeRelationshipState(entity, targetEntity, propName, EntityState.Deleted);
            }
        }

        protected override void ArrangeChildCollection(PropertyInfo propInfo, TEntity entity, DbContext dbContext)
        {
            var propValue = propInfo.GetValue(entity);
            DbSet childDbSet = GetChildDbSet(dbContext, propValue);

            var children = (IList)propValue;
            var curChildren = new Dictionary<int, Entity>();
            var newChildren = new List<Entity>();
            foreach (var child in children)
            {
                var childEntity = (Entity)child;
                if (childEntity.Id == default(int))
                {
                    childDbSet.Add(childEntity);
                    newChildren.Add(childEntity);
                }
                else
                {
                    curChildren.Add(childEntity.Id, childEntity);
                }
            }
            children.Clear();

            dbContext.Entry(entity).Collection(propInfo.Name).Load();
            children = (IList)propInfo.GetValue(entity);
            var oldChildren = new Entity[children.Count];
            children.CopyTo(oldChildren, 0);

            foreach (var child in oldChildren)
            {
                if (curChildren.ContainsKey(child.Id))
                {
                    dbContext.Entry(child).State = EntityState.Modified;
                }
                else
                {
                    childDbSet.Remove(child);
                }
            }

            foreach (var newChild in newChildren)
            {
                children.Add(newChild);
            }
        }
    }
}