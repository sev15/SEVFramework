using Moq;
using NUnit.Framework;
using SEV.DI;
using SEV.Domain.Services;

namespace SEV.DAL.EF.DI.Tests
{
    [TestFixture]
    public class DIContainerExtentionsTests
    {
        private Mock<IDIContainer> m_containerMock;
        private IDIContainer m_container;

        #region SetUp

        [SetUp]
        public void Init()
        {
            m_container = CreateDIContainer();
        }

        private IDIContainer CreateDIContainer()
        {
            var containerFactoryMock = new Mock<IDIContainerFactory>();
            m_containerMock = new Mock<IDIContainer>();
            containerFactoryMock.Setup(x => x.CreateContainer(false)).Returns(m_containerMock.Object);
            var configuration = DIConfiguration.Create(containerFactoryMock.Object);

            return configuration.CreateDIContainer();
        }

        #endregion

        [Test]
        public void WhenCallRegisterDomainServices_ThenShouldCallRegisterOfDIContainerForIUnitOfWorkFactory()
        {
            m_container.RegisterDomainServices();

            m_containerMock.Verify(x => x.Register<IUnitOfWorkFactory, EFUnitOfWorkFactory>(), Times.Once);
        }

        [Test]
        public void WhenCallRegisterDomainServices_ThenShouldCallRegisterOfDIContainerForIRepositoryFactory()
        {
            m_container.RegisterDomainServices();

            m_containerMock.Verify(x => x.Register<IRepositoryFactory, EFRepositoryFactory>(), Times.Once);
        }
    }
}