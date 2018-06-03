namespace SEV.Domain.Services.Data
{
    public interface IRelationshipsStripper<in TEntity> where TEntity : class
    {
        void Strip(TEntity entity, SEV.Domain.Model.DomainEvent domainEvent);
        void UnStrip(TEntity entity);
    }
}