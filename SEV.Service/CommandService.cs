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
                var stripper = unitOfWork.RelationshipsStripper<T>();
                stripper.Strip(entity);
                newEntity = unitOfWork.Repository<T>().Insert(entity);
                unitOfWork.SaveChanges();
                stripper.UnStrip(newEntity);
                RaiseEvent(unitOfWork, newEntity, DomainEvent.Create);
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
                var stripper = unitOfWork.RelationshipsStripper<T>();
                stripper.Strip(entity);
                unitOfWork.Repository<T>().Remove(entity);
                unitOfWork.SaveChanges();
                stripper.UnStrip(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Delete);
            }
        }

        public void Update<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Update);
                var stripper = unitOfWork.RelationshipsStripper<T>();
                stripper.Strip(entity);
                unitOfWork.Repository<T>().Update(entity);
                unitOfWork.SaveChanges();
                stripper.UnStrip(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Update);
            }
        }

        public async Task<T> CreateAsync<T>(T entity) where T : Entity
        {
            T newEntity;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Create);
                var stripper = unitOfWork.RelationshipsStripper<T>();
                stripper.Strip(entity);
                newEntity = unitOfWork.Repository<T>().Insert(entity);
                await unitOfWork.SaveChangesAsync();
                stripper.UnStrip(newEntity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Create);
            }

            return newEntity;
        }

        public async Task DeleteAsync<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Delete);
                var stripper = unitOfWork.RelationshipsStripper<T>();
                stripper.Strip(entity);
                unitOfWork.Repository<T>().Remove(entity);
                await unitOfWork.SaveChangesAsync();
                stripper.UnStrip(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Delete);
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : Entity
        {
            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                ValidateEntity(entity, DomainEvent.Update);
                var stripper = unitOfWork.RelationshipsStripper<T>();
                stripper.Strip(entity);
                unitOfWork.Repository<T>().Update(entity);
                await unitOfWork.SaveChangesAsync();
                stripper.UnStrip(entity);
                RaiseEvent(unitOfWork, entity, DomainEvent.Update);
            }
        }
    }
}