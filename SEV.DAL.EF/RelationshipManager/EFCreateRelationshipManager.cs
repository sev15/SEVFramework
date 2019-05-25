using SEV.Domain.Model;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;

namespace SEV.DAL.EF
{
    internal class EFCreateRelationshipManager<TEntity> : EFRelationshipManager<TEntity> where TEntity : Entity
    {
        public EFCreateRelationshipManager(IDbContext context, IReferenceContainer container)
            : base(context, container)
        {
            AttachEntity = false;
        }

        protected override void ArrangeChildCollection(KeyValuePair<PropertyInfo, ICollection> collectionInfo,
            TEntity entity, DbContext dbContext)
        {
            DbSet childDbSet = GetChildDbSet(dbContext, collectionInfo.Value);

            foreach (var child in collectionInfo.Value)
            {
                ArrangeChildRelationships((Entity)child);
                childDbSet.Add(child);
            }
        }
    }
}