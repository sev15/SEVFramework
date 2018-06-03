using System;
using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Services;
using SEV.Domain.Services.Data;
using SEV.Domain.Services.Logic;

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
        public void WhenCallRelationshipsStripper_ThenShouldReturnInstanceOfEFRelationshipsStripper()
        {
            var result = m_unitOfWork.RelationshipsStripper<Entity>();

            Assert.That(result, Is.InstanceOf<EFRelationshipsStripper<Entity>>());
        }

        [Test]
        public void WhenCallDomainEventsAggregator_ThenShouldReturnInstanceOfDomainEventsAggregator()
        {
            var result = m_unitOfWork.DomainEventsAggregator();

            Assert.That(result, Is.InstanceOf<DomainEventsAggregator>());
        }

        [Test]
        public void WhenCallRelationshipsLoader_ThenShouldReturnInstanceOfEFRelationshipsLoader()
        {
            var result = m_unitOfWork.RelationshipsLoader<Entity>();

            Assert.That(result, Is.InstanceOf<EFRelationshipsLoader<Entity>>());
        }

        [Test]
        public void WhenCallCreateDomainQueryHandler_ThenShouldCallGetInstanceWithKeyOfServiceLocatorForIDomainQueryHandlerFactory()
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
        public void GivenTypeOfQueryResultIsRight_WhenCallCreateDomainQueryHandler_ThenShouldReturnDomainQueryHandler()
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
        public void GivenTypeOfQueryResultIsWrong_WhenCallCreateDomainQueryHandler_ThenShouldThrowInvalidOperationException()
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
        public void WhenCallSaveChangesAsync_ThenShouldCallSaveChangesAsyncOfDbContext()
        {
            m_unitOfWork.SaveChangesAsync();

            m_dbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);
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