
namespace SEV.Domain.Services
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}