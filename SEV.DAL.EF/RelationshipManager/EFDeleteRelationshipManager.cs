using SEV.Domain.Model;
using System.Collections;
using System.Data.Entity;
using System.Reflection;

namespace SEV.DAL.EF
{
    internal class EFDeleteRelationshipManager<TEntity> : EFRelationshipManager<TEntity> where TEntity : Entity
    {
        public EFDeleteRelationshipManager(IDbContext context, IReferenceContainer container)
            : base(context, container)
        {
        }

        public override void PrepareRelationships(TEntity entity)
        {
            ArrangeRelationships(entity, true);
        }

        protected override void ArrangeChildCollection(PropertyInfo propInfo, TEntity entity, DbContext dbContext)
        {
            var propValue = propInfo.GetValue(entity);
            DbSet childDbSet = GetChildDbSet(dbContext, propValue);
            ((IList)propValue).Clear();

            dbContext.Entry(entity).Collection(propInfo.Name).Load();
            var children = (IList)propInfo.GetValue(entity);
            var oldChildren = new Entity[children.Count];
            children.CopyTo(oldChildren, 0);

            foreach (var child in oldChildren)
            {
                childDbSet.Remove(child);
            }
        }
    }
}