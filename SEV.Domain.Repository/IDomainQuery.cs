namespace SEV.Domain.Repository
{
    public interface IDomainQuery
    {
        dynamic this[string key] { get; set; }
    }
}