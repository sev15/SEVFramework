namespace SEV.Domain.Services.Data
{
    public interface IDomainQueryHandler
    {
    }

    public interface IDomainQueryHandler<out TResult> : IDomainQueryHandler
    {
        TResult Handle(System.Collections.Generic.IDictionary<string, dynamic> queryParams);
    }
}