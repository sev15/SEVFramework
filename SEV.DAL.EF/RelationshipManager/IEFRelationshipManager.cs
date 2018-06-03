using SEV.Domain.Model;

namespace SEV.DAL.EF
{
    public interface IEFRelationshipManager<in TEntity> where TEntity : Entity
    {
        void PrepareRelationships(TEntity entity);
        void RestoreRelationships(TEntity entity);
    }
}