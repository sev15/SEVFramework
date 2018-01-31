using SEV.Domain.Model;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace SEV.DAL.EF
{
    public class EFRelatedEntitiesStateAdjuster : IRelatedEntitiesStateAdjuster
    {
        private readonly IDbContext m_context;

        public EFRelatedEntitiesStateAdjuster(IDbContext context)
        {
            m_context = context;
        }

        public void AttachRelatedEntities<TEntity>(TEntity entity) where TEntity : Entity
        {
            PropertyInfo[] relatedEntitiesProperties =
                    typeof(TEntity).GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(Entity))).ToArray();
            if (!relatedEntitiesProperties.Any())
            {
                return;
            }

            var dbContext = (DbContext)m_context;

            foreach (var propertyInfo in relatedEntitiesProperties)
            {
                var relatedEntity = propertyInfo.GetValue(entity);
                if (relatedEntity != null && (dbContext.Entry(relatedEntity).State == EntityState.Detached))
                {
                    var relatedEntitySet = dbContext.Set(propertyInfo.PropertyType);
                    relatedEntitySet.Attach(relatedEntity);
                    dbContext.Entry(relatedEntity).State = EntityState.Unchanged;
                }
            }
        }
    }
}