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
        private Dictionary<int, Entity> m_currentChildren;

        public EFUpdateRelationshipManager(IDbContext context, IReferenceContainer container)
            : base(context, container)
        {
            AttachEntity = true;
        }

        protected override void ArrangeEntityRelationship(PropertyInfo propInfo, Entity entity, DbContext dbContext)
        {
            string propName = propInfo.Name;
            var relatedEntity = (Entity)propInfo.GetValue(entity);

            var stateManager = ((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager;
            var parentRelationship = stateManager.GetRelationshipManager(entity).GetAllRelatedEnds()
                                                 .OfType<EntityReference>()
                                                 .First(re => re.RelationshipName.EndsWith(propName));
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
                    return;
                }
                stateManager.ChangeRelationshipState(entity, relatedEntity, propName, EntityState.Unchanged);
            }
            else if (parentRelationshipId.HasValue)
            {
                var targetEntity = (Entity)propInfo.GetValue(entity);
                stateManager.ChangeRelationshipState(entity, targetEntity, propName, EntityState.Deleted);
            }
        }

        protected override void ArrangeChildCollection(KeyValuePair<PropertyInfo, ICollection> collectionInfo,
            TEntity entity, DbContext dbContext)
        {
            m_currentChildren = new Dictionary<int, Entity>();
            var newChildren = new List<Entity>();
            foreach (var child in collectionInfo.Value)
            {
                var childEntity = (Entity)child;
                if (childEntity.Id == default(int))
                {
                    newChildren.Add(childEntity);
                }
                else
                {
                    m_currentChildren.Add(childEntity.Id, childEntity);
                }
            }

            DbSet childDbSet = GetChildDbSet(dbContext, collectionInfo.Value);
            dbContext.Entry(entity).Collection(collectionInfo.Key.Name).Load();
            var oldChildren = ((IEnumerable<Entity>)collectionInfo.Key.GetValue(entity)).ToArray();

            foreach (var child in oldChildren)
            {
                if (m_currentChildren.ContainsKey(child.Id))
                {
                    dbContext.Entry(child).State = EntityState.Detached;
                    var currentChild = m_currentChildren[child.Id];
                    childDbSet.Attach(currentChild);
                    dbContext.Entry(currentChild).State = EntityState.Modified;
                    ArrangeChildRelationships(currentChild);
                }
                else
                {
                    ArrangeChildRelationships(child);
                    childDbSet.Remove(child);
                }
            }
            collectionInfo.Key.SetValue(entity, collectionInfo.Value);

            foreach (var newChild in newChildren)
            {
                ArrangeChildRelationships(newChild);
                childDbSet.Add(newChild);
            }
        }

        protected override void ArrangeChildRelationship(PropertyInfo propInfo, Entity entity, DbContext dbContext)
        {
            if (m_currentChildren.ContainsKey(entity.Id))
            {
                ArrangeEntityRelationship(propInfo, entity, dbContext);
            }
            else
            {
                base.ArrangeEntityRelationship(propInfo, entity, dbContext);
            }
        }
    }
}