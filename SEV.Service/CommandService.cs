using SEV.Common;
using SEV.Domain.Model;
using SEV.Domain.Services;
using SEV.Domain.Services.Logic;
using SEV.Service.Contract;
using System.Linq;
using System.Threading.Tasks;

namespace SEV.Service
{
    internal class CommandService : Service, ICommandService
    {
        private readonly IValidationService m_validationService;

        public CommandService(IUnitOfWorkFactory factory, IValidationService validationService) : base(factory)
        {
            m_validationService = validationService;
        }

        public T Create<T>(T entity) where T : Entity
        {
            T newEntity;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Create);
                newEntity = unitOfWork.Repository<T>().Insert(entity);
                RaiseEvent<T>(unitOfWork, entity, DomainEvent.Create);
                unitOfWork.SaveChanges();
            }

            return newEntity;
        }

        private void ValidateEntity(Entity entity, DomainEvent domainEvent)
        {
            var results = m_validationService.ValidateEntity(entity, domainEvent);
            if (results.Any())
            {
                throw new DomainValidationException(results);
            }
        }

        private void RaiseEvent<T>(IUnitOfWork unitOfWork, T entity, DomainEvent domainEvent) where T : Entity
        {
            unitOfWork.DomainEventsAggregator().RaiseEvent(new DomainEventArgs<T>(entity, domainEvent, unitOfWork));
        }

        public void Delete<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Delete);
                unitOfWork.Repository<T>().Remove(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Delete);
                unitOfWork.SaveChanges();
            }
        }

        public void Update<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Update);
                unitOfWork.Repository<T>().Update(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Update);
                unitOfWork.SaveChanges();
            }
        }

        public async Task<T> CreateAsync<T>(T entity) where T : Entity
        {
            T newEntity;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Create);
                newEntity = unitOfWork.Repository<T>().Insert(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Create);
                await unitOfWork.SaveChangesAsync();
            }

            return newEntity;
        }

        public async Task DeleteAsync<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Delete);
                unitOfWork.Repository<T>().Remove(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Delete);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Update);
                unitOfWork.Repository<T>().Update(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Update);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}