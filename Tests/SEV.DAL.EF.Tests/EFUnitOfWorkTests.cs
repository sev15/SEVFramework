using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Repository;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFUnitOfWorkTests
    {
        private Mock<IDbContext> m_dbContextMock;
        private IUnitOfWork m_unitOfWork;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_unitOfWork = new EFUnitOfWork(m_dbContextMock.Object);
        }

        private void InitMocks()
        {
            m_dbContextMock = new Mock<IDbContext>();
        }

        #endregion

        [Test]
        public void WhenCallRepository_ThenShouldReturnInstanceOfEFRepository()
        {
            var result = m_unitOfWork.Repository<Entity>();

            Assert.That(result, Is.InstanceOf<EFRepository<Entity>>());
        }

        [Test]
        public void WhenCallRelationshipManager_ThenShouldReturnInstanceOfEFRelationshipManager()
        {
            var result = m_unitOfWork.RelationshipManager<Entity>();

            Assert.That(result, Is.InstanceOf<EFRelationshipManager<Entity>>());
        }

        [Test]
        public void WhenCallDomainQueryProvider_ThenShouldCallGetInstanceOfServiceLocatorForIDomainQueryHandlerFactory()
        {
            var queryHandlerFactoryMock = new Mock<IDomainQueryHandlerFactory>();
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<IDomainQueryHandlerFactory>())
                              .Returns(queryHandlerFactoryMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            m_unitOfWork.DomainQueryProvider();

           serviceLocatorMock.Verify(x => x.GetInstance<IDomainQueryHandlerFactory>(), Times.Once);
        }

        [Test]
        public void WhenCallDomainQueryProvider_ThenShouldReturnInstanceOfDomainQueryProvider()
        {
            ServiceLocator.SetLocatorProvider(() => new Mock<IServiceLocator>().Object);

            var result = m_unitOfWork.DomainQueryProvider();

            Assert.That(result, Is.InstanceOf<DomainQueryProvider>());
        }

        [Test]
        public void WhenCallSaveChanges_ThenShouldCallSaveChangesOfDbContext()
        {
            m_unitOfWork.SaveChanges();

            m_dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void WhenCallDispose_ThenShouldCallDisposeOfDbContext()
        {
            m_unitOfWork.Dispose();

            m_dbContextMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void GivenDisposeIsInvoked_WhenCallDispose_ThenShouldNotCallDisposeOfDbContext()
        {
            m_unitOfWork.Dispose();

            m_unitOfWork.Dispose();

            m_dbContextMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}