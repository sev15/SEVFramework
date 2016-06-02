using SEV.Domain.Model;

namespace SEV.Service.Contract
{
    public interface ICommandService
    {
        T Create<T>(T entity) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
        void Update<T>(T entity) where T : Entity;
    }
}