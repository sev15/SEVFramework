using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Repository;
using System.Data.Entity;
using System.Linq;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFRepositoryTests
    {
        private Mock<IDbContext> m_dbContextMock;
        private Mock<IDbSet<Entity>> m_dbSetMock;
        private IRepository<Entity> m_repository;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_repository = new EFRepository<Entity>(m_dbContextMock.Object);
        }

        private void InitMocks()
        {
            m_dbSetMock = new Mock<IDbSet<Entity>>();
            m_dbContextMock = new Mock<IDbContext>();
            m_dbContextMock.Setup(x => x.Set<Entity>()).Returns(m_dbSetMock.Object);
        }

        #endregion

        [Test]
        public void WhenCreateEFRepositoryObject_ThenShouldCallSetOfDbContext()
        {
            m_dbContextMock.Verify(x => x.Set<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallAll_ThenShouldReturnEntityCollectionProvidedByDbSet()
        {
            var queryable = Enumerable.Range(1, 3).Select(x =>
            {
                var entity = new Mock<Entity> { CallBase = true }.Object;
                entity.Id = x;
                return entity;
            }).AsQueryable();
            m_dbSetMock.Setup(x => x.Provider).Returns(queryable.Provider);
            m_dbSetMock.Setup(x => x.Expression).Returns(queryable.Expression);
            m_dbSetMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            m_dbSetMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator);

            var result = m_repository.All();

            Assert.That(result, Is.EquivalentTo(queryable));
        }

        [Test]
        public void WhenCallGetById_ThenShouldCallFindOfDbSet()
        {
            const int id = 3;

            m_repository.GetById(id);

            m_dbSetMock.Verify(x => x.Find(id), Times.Once);
        }

        [Test]
        public void GivenSuppliedIdIsStringValue_WhenCallGetById_ThenShouldCallFindOfDbSetWithIdTransformedInIntValue()
        {
            const int id = 2;
            string idStr = id.ToString();

            m_repository.GetById(idStr);

            m_dbSetMock.Verify(x => x.Find(id), Times.Once);
        }

        [Test]
        public void WhenCallGetByIdList_ThenShouldReturnEntityCollectionProvidedByDbSetFilteredByProvidedIdList()
        {
            var queryable = Enumerable.Range(1, 5).Select(x =>
            {
                var entity = new Mock<Entity> { CallBase = true }.Object;
                entity.Id = x;
                return entity;
            }).AsQueryable();
            m_dbSetMock.Setup(x => x.Provider).Returns(queryable.Provider);
            m_dbSetMock.Setup(x => x.Expression).Returns(queryable.Expression);
            m_dbSetMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            m_dbSetMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator);

            var result = m_repository.GetByIdList(new[] { "1", "4" });

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.FirstOrDefault(x => x.Id == 1), Is.Not.Null);
            Assert.That(result.FirstOrDefault(x => x.Id == 4), Is.Not.Null);
        }

        [Test]
        public void WhenCallQuery_ThenShouldReturnInstanceOfRepositoryQuery()
        {
            var result = m_repository.Query();

            Assert.That(result, Is.InstanceOf<RepositoryQuery<Entity>>());
        }

        [Test]
        public void WhenCallCreateQuery_ThenShouldReturnEntityCollectionProvidedByDbSet()
        {
            var queryable = Enumerable.Range(1, 4).Select(x =>
            {
                var entity = new Mock<Entity> { CallBase = true }.Object;
                entity.Id = x;
                return entity;
            }).AsQueryable();
            m_dbSetMock.Setup(x => x.Provider).Returns(queryable.Provider);
            m_dbSetMock.Setup(x => x.Expression).Returns(queryable.Expression);
            m_dbSetMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            m_dbSetMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator);
            var query = m_repository.Query();

            var result = query.Get();

            Assert.That(result, Is.EquivalentTo(queryable));
        }

        [Test]
        public void WhenCallInsert_ThenShouldCallGetInstanceOfServiceLocatorForIRelatedEntitiesStateAdjuster()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<IRelatedEntitiesStateAdjuster>())
                              .Returns(new Mock<IRelatedEntitiesStateAdjuster>().Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
            var entity = new Mock<Entity>().Object;

            m_repository.Insert(entity);

            serviceLocatorMock.Verify(x => x.GetInstance<IRelatedEntitiesStateAdjuster>(), Times.Once);
        }

        [Test]
        public void WhenCallInsert_ThenShouldCallAttachRelatedEntitiesOfRelatedEntitiesStateAdjuster()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();
            var adjusterMock = new Mock<IRelatedEntitiesStateAdjuster>();
            serviceLocatorMock.Setup(x => x.GetInstance<IRelatedEntitiesStateAdjuster>()).Returns(adjusterMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
            var entity = new Mock<Entity>().Object;

            m_repository.Insert(entity);

            adjusterMock.Verify(x => x.AttachRelatedEntities(entity, m_dbContextMock.Object), Times.Once);
        }

        [Test]
        public void WhenCallInsert_ThenShouldCallAddOfDbSet()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<IRelatedEntitiesStateAdjuster>())
                              .Returns(new Mock<IRelatedEntitiesStateAdjuster>().Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
            var entity = new Mock<Entity>().Object;

            m_repository.Insert(entity);

            m_dbSetMock.Verify(x => x.Add(entity), Times.Once);
        }

        [Test]
        public void WhenCallRemove_ThenShouldCallRemoveOfDbSet()
        {
            var entity = new Mock<Entity>().Object;
            m_dbContextMock.Setup(x => x.GetEntityState(entity)).Returns(EntityState.Unchanged);

            m_repository.Remove(entity);

            m_dbSetMock.Verify(x => x.Remove(entity), Times.Once);
        }

        [Test]
        public void GivenProvidedEntityIsNotAttachedToDbContext_WhenCallRemove_ThenShouldCallAttachOfDbSet()
        {
            var entity = new Mock<Entity>().Object;
            m_dbContextMock.Setup(x => x.GetEntityState(entity)).Returns(EntityState.Detached);

            m_repository.Remove(entity);

            m_dbSetMock.Verify(x => x.Attach(entity), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldSetEntityStateToModified()
        {
            MockServiceLocator();
            var entity = new Mock<Entity>().Object;
            m_dbContextMock.Setup(x => x.GetEntityState(entity)).Returns(EntityState.Unchanged);

            m_repository.Update(entity);

            m_dbContextMock.Verify(x => x.SetEntityState(entity, EntityState.Modified), Times.Once);
        }

        private Mock<IServiceLocator> MockServiceLocator()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<EntityAssociationsUpdater>(typeof(Entity).FullName))
                              .Throws<ActivationException>();
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            return serviceLocatorMock;
        }

        [Test]
        public void GivenProvidedEntityIsNotAttachedToDbContext_WhenCallUpdate_ThenShouldCallAttachOfDbSet()
        {
            MockServiceLocator();
            var entity = new Mock<Entity>().Object;
            m_dbContextMock.Setup(x => x.GetEntityState(entity)).Returns(EntityState.Detached);

            m_repository.Update(entity);

            m_dbSetMock.Verify(x => x.Attach(entity), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldCallGetInstanceOfServiceLocatorForEntityAssociationsUpdater()
        {
            var serviceLocatorMock = MockServiceLocator();
            var entity = new Mock<Entity>().Object;

            m_repository.Update(entity);

            serviceLocatorMock.Verify(x => x.GetInstance(typeof(EntityAssociationsUpdater),
                                                            typeof(Entity).FullName), Times.Once);
        }

        [Test]
        public void GivenEntityAssociationsUpdaterIsRegistered_WhenCallUpdate_ThenShouldCallUpdateAssociationsOfEntityAssociationsUpdater()
        {
            var serviceLocatorMock = MockServiceLocator();
            var associationsUpdaterMock = new Mock<EntityAssociationsUpdater>();
            serviceLocatorMock.Setup(x => x.GetInstance(typeof(EntityAssociationsUpdater), typeof(Entity).FullName))
                              .Returns(associationsUpdaterMock.Object);
            var entity = new Mock<Entity>().Object;

            m_repository.Update(entity);

            associationsUpdaterMock.Verify(x => x.UpdateAssociations(entity, m_dbContextMock.Object), Times.Once);
        }
    }
}