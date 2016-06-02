namespace SEV.Domain.Repository
{
    public interface IDomainQueryHandler<out TResult>
    {
        TResult Handle(IDomainQuery query);
    }
}