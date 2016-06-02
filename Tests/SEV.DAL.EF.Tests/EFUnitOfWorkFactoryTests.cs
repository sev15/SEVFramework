using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Domain.Repository;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFUnitOfWorkFactoryTests
    {
        private Mock<IDbContext> m_dbContextMock;
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private IUnitOfWorkFactory m_factory;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_factory = new EFUnitOfWorkFactory();
        }

        private void InitMocks()
        {
            m_dbContextMock = new Mock<IDbContext>();
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<IDbContext>()).Returns(m_dbContextMock.Object);
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
        }

        #endregion

        [Test]
        public void WhenCallCreate_ThenShouldCallGetInstanceOfServiceLocatorForIDbContext()
        {
            m_factory.Create();

            m_serviceLocatorMock.Verify(x => x.GetInstance<IDbContext>(), Times.Once);
        }

        [Test]
        public void WhenCallCreate_ThenShouldReturnInstanceOfEFUnitOfWork()
        {
            var result = m_factory.Create();

            Assert.That(result, Is.InstanceOf<EFUnitOfWork>());
        }
    }
}