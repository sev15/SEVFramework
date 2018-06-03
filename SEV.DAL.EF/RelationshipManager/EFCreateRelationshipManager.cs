using SEV.Domain.Model;
using System.Collections;
using System.Data.Entity;
using System.Reflection;

namespace SEV.DAL.EF
{
    internal class EFCreateRelationshipManager<TEntity> : EFRelationshipManager<TEntity> where TEntity : Entity
    {
        public EFCreateRelationshipManager(IDbContext context, IReferenceContainer container)
            : base(context, container)
        {
        }

        public override void PrepareRelationships(TEntity entity)
        {
            ArrangeRelationships(entity);
        }

        protected override void ArrangeChildCollection(PropertyInfo propInfo, TEntity entity, DbContext dbContext)
        {
            var propValue = propInfo.GetValue(entity);
            DbSet childDbSet = GetChildDbSet(dbContext, propValue);

            foreach (var child in (ICollection)propValue)
            {
                childDbSet.Add(child);
            }
        }
    }
}