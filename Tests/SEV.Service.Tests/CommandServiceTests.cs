using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Repository;
using SEV.Service.Contract;

namespace SEV.Service.Tests
{
    [TestFixture]
    public class CommandServiceTests
    {
        private Mock<IUnitOfWorkFactory> m_unitOfWorkFactoryMock;
        private Mock<IUnitOfWork> m_unitOfWorkMock;
        private Entity m_entity;
        private Mock<IRepository<Entity>> m_entityRepositoryMock;
        private Mock<IRelationshipManager<Entity>> m_entityRelationshipManagerMock;
        private ICommandService m_service;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_service = new CommandService(m_unitOfWorkFactoryMock.Object);
        }

        private void InitMocks()
        {
            m_entity = new Mock<Entity>().Object;
            m_entityRepositoryMock = new Mock<IRepository<Entity>>();
            m_entityRepositoryMock.Setup(x => x.Insert(m_entity)).Returns(m_entity);
            m_entityRelationshipManagerMock = new Mock<IRelationshipManager<Entity>>();
            m_unitOfWorkMock = new Mock<IUnitOfWork>();
            m_unitOfWorkMock.Setup(x => x.Repository<Entity>()).Returns(m_entityRepositoryMock.Object);
            m_unitOfWorkMock.Setup(x => x.RelationshipManager<Entity>()).Returns(m_entityRelationshipManagerMock.Object);
            m_unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            m_unitOfWorkFactoryMock.Setup(x => x.Create()).Returns(m_unitOfWorkMock.Object);
        }

        #endregion

        [Test]
        public void WhenCallCreate_ThenShouldCreateUnitOfWork()
        {
            m_service.Create(m_entity);

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
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
        public void WhenCallCreate_ThenShouldCallRelationshipManagerOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.Create(m_entity);

            m_unitOfWorkMock.Verify(x => x.RelationshipManager<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldCallCreateRelatedEntitiesOfRelationshipManager()
        {
            var createdEntity = new Mock<Entity>().Object;
            m_entityRepositoryMock.Setup(x => x.Insert(m_entity)).Returns(createdEntity);

            m_service.Create(m_entity);

            m_entityRelationshipManagerMock.Verify(x => x.CreateRelatedEntities(m_entity, createdEntity), Times.Once);
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
        public void WhenCallUpdate_ThenShouldCallRelationshipManagerOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.Update(m_entity);

            m_unitOfWorkMock.Verify(x => x.RelationshipManager<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallUpdateRelatedEntitiesOfRelationshipManager()
        {
            m_service.Update(m_entity);

            m_entityRelationshipManagerMock.Verify(x => x.UpdateRelatedEntities(m_entity), Times.Once);
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
        public void WhenCallCreateAsync_ThenShouldCallCreateRelatedEntitiesOfRelationshipManager()
        {
            var createdEntity = new Mock<Entity>().Object;
            m_entityRepositoryMock.Setup(x => x.Insert(m_entity)).Returns(createdEntity);

            m_service.CreateAsync(m_entity);

            m_entityRelationshipManagerMock.Verify(x => x.CreateRelatedEntities(m_entity, createdEntity), Times.Once);
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
        public void WhenCallUpdateAsync_ThenShouldCallUpdateRelatedEntitiesOfRelationshipManager()
        {
            m_service.UpdateAsync(m_entity);

            m_entityRelationshipManagerMock.Verify(x => x.UpdateRelatedEntities(m_entity), Times.Once);
        }

        [Test]
        public void WhenCallUpdateAsync_ThenShouldCallSaveChangesAsyncOfUnitOfWork()
        {
            m_service.UpdateAsync(m_entity);

            m_unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}