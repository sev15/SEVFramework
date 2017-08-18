namespace SEV.Domain.Repository
{
    public interface IDomainQueryHandler
    {
    }

    public interface IDomainQueryHandler<out TResult> : IDomainQueryHandler
    {
        TResult Handle(System.Collections.Generic.IDictionary<string, dynamic> queryParams);
    }
}