using SEV.Domain.Model;
using SEV.Domain.Services.Data;

namespace SEV.DAL.EF
{
    public class EFRelationshipsStripper<TEntity> : RelationshipsStripper<TEntity> where TEntity : Entity
    {
        private readonly IRelatedEntitiesStateAdjuster m_relatedEntitiesAdjuster;

        public EFRelationshipsStripper(IRelatedEntitiesStateAdjuster relatedEntitiesAdjuster)
        {
            m_relatedEntitiesAdjuster = relatedEntitiesAdjuster;
        }

        public override void Strip(TEntity entity)
        {
            base.Strip(entity);

            m_relatedEntitiesAdjuster.AttachRelatedEntities(entity);
        }
    }
}
