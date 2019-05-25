using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;

namespace SEV.DI.Web.Tests
{
    [TestFixture]
    public class DIContainerExtensionsTests
    {
        private Mock<IDIContainerWebConfigurator> m_webConfiguratorMock ;
        private Mock<IServiceLocator> m_serviceLocatorMock;
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
            m_webConfiguratorMock = new Mock<IDIContainerWebConfigurator>();
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<IDIContainerWebConfigurator>())
                                .Returns(m_webConfiguratorMock.Object);
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
            m_containerMock = new Mock<IDIContainer>();

            return m_containerMock.Object;
        }

        #endregion

        [Test]
        public void WhenCallGetWebConfigurator_ThenShouldCallGetInstanceOfServiceLocatorForIDIContainerWebConfigurator()
        {
            m_container.GetWebConfigurator();

            m_serviceLocatorMock.Verify(x => x.GetInstance<IDIContainerWebConfigurator>(), Times.Once);
        }

        [Test]
        public void WhenCallGetWebConfigurator_ThenShouldCallSetContainerOfDIContainerWebConfigurator()
        {
            m_container.GetWebConfigurator();

            m_webConfiguratorMock.Verify(x => x.SetContainer(m_container), Times.Once);
        }

        [Test]
        public void WhenCallGetWebConfigurator_ThenShouldReturnInstanceOfIDIContainerWebConfiguratorFromServiceLocator()
        {
            var result = m_container.GetWebConfigurator();

            Assert.That(result, Is.SameAs(m_webConfiguratorMock.Object));
        }
    }
}