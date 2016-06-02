
namespace SEV.Domain.Repository
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}