namespace SEV.Domain.Repository
{
    public interface IDomainQueryHandlerFactory
    {
        IDomainQueryHandler<TResult> CreateHandler<TResult>(string queryName); 
    }
}