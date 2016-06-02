namespace SEV.DAL.EF
{
    public interface IRelatedEntitiesStateAdjuster
    {
        void AttachRelatedEntities<TEntity>(TEntity entity, IDbContext context)
            where TEntity : SEV.Domain.Model.Entity;
    }
}
