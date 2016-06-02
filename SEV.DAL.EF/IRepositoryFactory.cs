namespace SEV.DAL.EF
{
    public interface IRepositoryFactory
    {
        dynamic Create(System.Type objectType, dynamic objectContext);
    }
}