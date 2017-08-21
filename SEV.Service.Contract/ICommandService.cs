using System.Threading.Tasks;
using SEV.Domain.Model;

namespace SEV.Service.Contract
{
    public interface ICommandService
    {
        T Create<T>(T entity) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
        void Update<T>(T entity) where T : Entity;

        Task<T> CreateAsync<T>(T entity) where T : Entity;
        Task DeleteAsync<T>(T entity) where T : Entity;
        Task UpdateAsync<T>(T entity) where T : Entity;
    }
}