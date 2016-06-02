namespace SEV.Domain.Repository
{
    public interface IRelatedEntitiesUpdater<TEntity> where TEntity : class
    {
        void Execute(EntityActionEventArgs<TEntity> createEntityEventArgs);

    }
}