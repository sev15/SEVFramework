using System.Threading.Tasks;
using SEV.Domain.Model;
using SEV.Domain.Repository;
using SEV.Service.Contract;

namespace SEV.Service
{
    internal class CommandService : Service, ICommandService
    {
        public CommandService(IUnitOfWorkFactory factory) : base(factory)
        {
        }

        public T Create<T>(T entity) where T : Entity
        {
            T newEntity;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                newEntity = unitOfWork.Repository<T>().Insert(entity);
                unitOfWork.RelationshipManager<T>().CreateRelatedEntities(entity, newEntity);
                unitOfWork.SaveChanges();
            }

            return newEntity;
        }

        public void Delete<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.Repository<T>().Remove(entity);
                unitOfWork.SaveChanges();
            }
        }

        public void Update<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.Repository<T>().Update(entity);
                unitOfWork.RelationshipManager<T>().UpdateRelatedEntities(entity);
                unitOfWork.SaveChanges();
            }
        }

        public async Task<T> CreateAsync<T>(T entity) where T : Entity
        {
            T newEntity;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                newEntity = unitOfWork.Repository<T>().Insert(entity);
                unitOfWork.RelationshipManager<T>().CreateRelatedEntities(entity, newEntity);
                await unitOfWork.SaveChangesAsync();
            }

            return newEntity;
        }

        public async Task DeleteAsync<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.Repository<T>().Remove(entity);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.Repository<T>().Update(entity);
                unitOfWork.RelationshipManager<T>().UpdateRelatedEntities(entity);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}