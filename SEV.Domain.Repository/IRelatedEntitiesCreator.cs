namespace SEV.Domain.Repository
{
    public interface IRelatedEntitiesCreator<TEntity> where TEntity : class
    {
        void Execute(CreateEntityEventArgs<TEntity> createEntityEventArgs);
    }
}