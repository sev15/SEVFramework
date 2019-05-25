using SEV.Domain.Model;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace SEV.DAL.EF
{
    internal class EFDeleteRelationshipManager<TEntity> : EFRelationshipManager<TEntity> where TEntity : Entity
    {
        public EFDeleteRelationshipManager(IDbContext context, IReferenceContainer container)
            : base(context, container)
        {
            AttachEntity = true;
        }

        protected override void ArrangeChildCollection(KeyValuePair<PropertyInfo, ICollection> collectionInfo,
            TEntity entity, DbContext dbContext)
        {
            DbSet childDbSet = GetChildDbSet(dbContext, collectionInfo.Value);
            dbContext.Entry(entity).Collection(collectionInfo.Key.Name).Load();
            var oldChildren = ((IEnumerable<Entity>)collectionInfo.Key.GetValue(entity)).ToArray();

            foreach (var child in oldChildren)
            {
                ArrangeChildRelationships(child);
                childDbSet.Remove(child);
            }
        }
    }
}