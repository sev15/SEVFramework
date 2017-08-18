using System;
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
        public void WhenCallDomainQueryProvider_ThenShouldCallGetInstanceWithKeyOfServiceLocatorForIDomainQueryHandlerFactory()
        {
            const string queryName = "testQueryName";
            var queryHandlerMock = new Mock<IDomainQueryHandler<bool>>();
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<IDomainQueryHandler>(queryName)).Returns(queryHandlerMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            m_unitOfWork.CreateDomainQueryHandler<bool>(queryName);

            serviceLocatorMock.Verify(x => x.GetInstance<IDomainQueryHandler>(queryName), Times.Once);
        }

        [Test]
        public void GivenTypeOfQueryResultIsRight_WhenCallDomainQueryProvider_ThenShouldReturnDomainQueryHandler()
        {
            const string queryName = "testQueryName";
            var queryHandlerMock = new Mock<IDomainQueryHandler<bool>>();
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<IDomainQueryHandler>(queryName)).Returns(queryHandlerMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            var result = m_unitOfWork.CreateDomainQueryHandler<bool>(queryName);

            Assert.That(result, Is.SameAs(queryHandlerMock.Object));
        }

        [Test]
        public void GivenTypeOfQueryResultIsWrong_WhenCallDomainQueryProvider_ThenShouldThrowInvalidOperationException()
        {
            const string queryName = "testQueryName";
            var queryHandlerMock = new Mock<IDomainQueryHandler<bool>>();
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<IDomainQueryHandler>(queryName)).Returns(queryHandlerMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            Assert.That(() => m_unitOfWork.CreateDomainQueryHandler<int>(queryName), Throws.InstanceOf<InvalidOperationException>());
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