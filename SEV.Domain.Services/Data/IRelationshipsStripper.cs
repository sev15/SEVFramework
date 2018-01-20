namespace SEV.Domain.Services.Data
{
    public interface IRelationshipsStripper<in TEntity> where TEntity : class
    {
        void Strip(TEntity entity);
        void UnStrip(TEntity entity);
    }
}