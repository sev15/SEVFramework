using System.ComponentModel.DataAnnotations;
using Moq;
using NUnit.Framework;
using SEV.Common;
using SEV.Domain.Model;
using SEV.Domain.Services;
using SEV.Domain.Services.Data;
using SEV.Domain.Services.Logic;
using SEV.Service.Contract;

namespace SEV.Service.Tests
{
    [TestFixture]
    public class CommandServiceTests
    {
        private Mock<IUnitOfWorkFactory> m_unitOfWorkFactoryMock;
        private Mock<IUnitOfWork> m_unitOfWorkMock;
        private Mock<IValidationService> m_validationServiceMock;
        private Entity m_entity;
        private Mock<IRepository<Entity>> m_entityRepositoryMock;
        private Mock<IDomainEventsAggregator> m_eventsAggregatorMock;
        private ICommandService m_service;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_service = new CommandService(m_unitOfWorkFactoryMock.Object, m_validationServiceMock.Object);
        }

        private void InitMocks()
        {
            m_entity = new Mock<Entity>().Object;
            m_entityRepositoryMock = new Mock<IRepository<Entity>>();
            m_entityRepositoryMock.Setup(x => x.Insert(m_entity)).Returns(m_entity);
            m_unitOfWorkMock = new Mock<IUnitOfWork>();
            m_unitOfWorkMock.Setup(x => x.Repository<Entity>()).Returns(m_entityRepositoryMock.Object);
            m_eventsAggregatorMock = new Mock<IDomainEventsAggregator>();
            m_unitOfWorkMock.Setup(x => x.DomainEventsAggregator()).Returns(m_eventsAggregatorMock.Object);
            m_unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            m_unitOfWorkFactoryMock.Setup(x => x.Create()).Returns(m_unitOfWorkMock.Object);
            m_validationServiceMock = new Mock<IValidationService>();
            m_validationServiceMock.Setup(x => x.ValidateEntity(It.IsAny<Entity>(), It.IsAny<DomainEvent>()))
                                   .Returns(new ValidationResult[0]);
        }

        #endregion

        [Test]
        public void WhenCallCreate_ThenShouldCreateUnitOfWork()
        {
            m_service.Create(m_entity);

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallValidateEntityOfValidationServiceForSpecifiedEntityAndCreateDomainEvent()
        {
            m_service.Create(m_entity);

            m_validationServiceMock.Verify(x => x.ValidateEntity(m_entity, DomainEvent.Create), Times.Once);
        }

        [Test]
        public void GivenValidationServiceReturnsSomeValidationResults_WhenCallCreate_ThenShouldThrowDomainValidationException()
        {
            m_validationServiceMock.Setup(x => x.ValidateEntity(m_entity, DomainEvent.Create))
                                   .Returns(new[] { new ValidationResult("test message") });

            Assert.That(() => m_service.Create(m_entity), Throws.TypeOf<DomainValidationException>());
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallRepositoryOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.Create(m_entity);

            m_unitOfWorkMock.Verify(x => x.Repository<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallInsertOfEntityRepository()
        {
            m_service.Create(m_entity);

            m_entityRepositoryMock.Verify(x => x.Insert(m_entity), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallDomainEventsAggregatorOfUnitOfWork()
        {
            m_service.Create(m_entity);

            m_unitOfWorkMock.Verify(x => x.DomainEventsAggregator(), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallRaiseEventOfDomainEventsAggregatorForSpecifiedEntityAndCreateDomainEvent()
        {
            m_service.Create(m_entity);

// ReSharper disable PossibleUnintendedReferenceComparison
            m_eventsAggregatorMock.Verify(x => x.RaiseEvent(It.Is<DomainEventArgs<Entity>>(y => y.Entity == m_entity &&
                                y.Event == DomainEvent.Create && y.UnitOfWork == m_unitOfWorkMock.Object)), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallSaveChangesOfUnitOfWork()
        {
            m_service.Create(m_entity);

            m_unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldReturnNewEntityProvidedByEntityRepository()
        {
            var result = m_service.Create(m_entity);

            Assert.That(result, Is.SameAs(m_entity));
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallDisposeOfUnitOfWork()
        {
            m_service.Create(m_entity);

            m_unitOfWorkMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void WhenCallDelete_ThenShouldCreateUnitOfWork()
        {
            m_service.Delete(m_entity);

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public void WhenCallDelete_ThenShouldCallValidateEntityOfValidationServiceForSpecifiedEntityAndDeleteDomainEvent()
        {
            m_service.Delete(m_entity);

            m_validationServiceMock.Verify(x => x.ValidateEntity(m_entity, DomainEvent.Delete), Times.Once);
        }

        [Test]
        public void GivenValidationServiceReturnsSomeValidationResults_WhenCallDelete_ThenShouldThrowDomainValidationException()
        {
            m_validationServiceMock.Setup(x => x.ValidateEntity(m_entity, DomainEvent.Delete))
                                   .Returns(new[] { new ValidationResult("test message") });

            Assert.That(() => m_service.Delete(m_entity), Throws.TypeOf<DomainValidationException>());
        }

        [Test]
        public void WhenCallDelete_ThenShouldCallRepositoryOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.Delete(m_entity);

            m_unitOfWorkMock.Verify(x => x.Repository<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallDelete_ThenShouldCallRemoveOfEntityRepository()
        {
            m_service.Delete(m_entity);

            m_entityRepositoryMock.Verify(x => x.Remove(m_entity), Times.Once);
        }

        [Test]
        public void WhenCallDelete_ThenShouldCallDomainEventsAggregatorOfUnitOfWork()
        {
            m_service.Delete(m_entity);

            m_unitOfWorkMock.Verify(x => x.DomainEventsAggregator(), Times.Once);
        }

        [Test]
        public void WhenCallDelete_ThenShouldCallRaiseEventOfDomainEventsAggregatorForSpecifiedEntityAndDeleteDomainEvent()
        {
            m_service.Delete(m_entity);

            m_eventsAggregatorMock.Verify(x => x.RaiseEvent(It.Is<DomainEventArgs<Entity>>(y => y.Entity == m_entity &&
                            y.Event == DomainEvent.Delete && y.UnitOfWork == m_unitOfWorkMock.Object)), Times.Once);
        }

        [Test]
        public void WhenCallDelete_ThenShouldCallSaveChangesOfUnitOfWork()
        {
            m_service.Delete(m_entity);

            m_unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void WhenCallDelete_ThenShouldCallDisposeOfUnitOfWork()
        {
            m_service.Delete(m_entity);

            m_unitOfWorkMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCreateUnitOfWork()
        {
            m_service.Update(m_entity);

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallValidateEntityOfValidationServiceForSpecifiedEntityAndUpdateDomainEvent()
        {
            m_service.Update(m_entity);

            m_validationServiceMock.Verify(x => x.ValidateEntity(m_entity, DomainEvent.Update), Times.Once);
        }

        [Test]
        public void GivenValidationServiceReturnsSomeValidationResults_WhenCallUpdate_ThenShouldThrowDomainValidationException()
        {
            m_validationServiceMock.Setup(x => x.ValidateEntity(m_entity, DomainEvent.Update))
                                   .Returns(new[] { new ValidationResult("test message") });

            Assert.That(() => m_service.Update(m_entity), Throws.TypeOf<DomainValidationException>());
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallRepositoryOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.Update(m_entity);

            m_unitOfWorkMock.Verify(x => x.Repository<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallUpdateOfEntityRepository()
        {
            m_service.Update(m_entity);

            m_entityRepositoryMock.Verify(x => x.Update(m_entity), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallDomainEventsAggregatorOfUnitOfWork()
        {
            m_service.Update(m_entity);

            m_unitOfWorkMock.Verify(x => x.DomainEventsAggregator(), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallRaiseEventOfDomainEventsAggregatorForSpecifiedEntityAndUpdateDomainEvent()
        {
            m_service.Update(m_entity);

            m_eventsAggregatorMock.Verify(x => x.RaiseEvent(It.Is<DomainEventArgs<Entity>>(y => y.Entity == m_entity &&
                            y.Event == DomainEvent.Update && y.UnitOfWork == m_unitOfWorkMock.Object)), Times.Once);
// ReSharper restore PossibleUnintendedReferenceComparison
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallSaveChangesOfUnitOfWork()
        {
            m_service.Update(m_entity);

            m_unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallDisposeOfUnitOfWork()
        {
            m_service.Update(m_entity); 

            m_unitOfWorkMock.Verify(x => x.Dispose(), Times.Once);
        }

        // INFO : the unit tests for async methods are almost the same as for corresponding sync methods

        [Test]
        public void WhenCallCreateAsync_ThenShouldCallInsertOfEntityRepository()
        {
            m_service.Create(m_entity);

            m_entityRepositoryMock.Verify(x => x.Insert(m_entity), Times.Once);
        }

        [Test]
        public void WhenCallCreateAsync_ThenShouldCallSaveChangesAsyncOfUnitOfWork()
        {
            m_service.CreateAsync(m_entity);

            m_unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async void WhenCallCreateAsync_ThenShouldReturnNewEntityProvidedByEntityRepository()
        {
            var result = await m_service.CreateAsync(m_entity);

            Assert.That(result, Is.SameAs(m_entity));
        }

        [Test]
        public void WhenCallDeleteAsync_ThenShouldCallRemoveOfEntityRepository()
        {
            m_service.DeleteAsync(m_entity);

            m_entityRepositoryMock.Verify(x => x.Remove(m_entity), Times.Once);
        }

        [Test]
        public void WhenCallDeleteAsync_ThenShouldCallSaveChangesAsyncOfUnitOfWork()
        {
            m_service.DeleteAsync(m_entity);

            m_unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void WhenCallUpdateAsync_ThenShouldCallUpdateOfEntityRepository()
        {
            m_service.UpdateAsync(m_entity);

            m_entityRepositoryMock.Verify(x => x.Update(m_entity), Times.Once);
        }

        [Test]
        public void WhenCallUpdateAsync_ThenShouldCallSaveChangesAsyncOfUnitOfWork()
        {
            m_service.UpdateAsync(m_entity);

            m_unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}