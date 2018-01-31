namespace SEV.DAL.EF
{
    public interface IRelatedEntitiesStateAdjuster
    {
        void AttachRelatedEntities<TEntity>(TEntity entity) where TEntity : SEV.Domain.Model.Entity;
    }
}
