using Moq;
using NUnit.Framework;

namespace SEV.DI.Tests
{
    [TestFixture]
    public class DIConfigurationTests
    {
        private Mock<IDIContainerFactory> m_containerFactoryMock;
        private DIConfiguration m_configuration;

        [SetUp]
        public void Init()
        {
            m_containerFactoryMock = new Mock<IDIContainerFactory>();

            m_configuration = DIConfiguration.Create(m_containerFactoryMock.Object);
        }

        [Test]
        public void WhenCallCreate_ThenShouldReturnInstanceOfDIConfiguration()
        {
            Assert.That(m_configuration, Is.Not.Null);
        }

        [Test]
        public void WhenCallCreateDIContainer_ThenShouldCallCreateContainerOfDIContainerFactory()
        {
            var containerMock = new Mock<IDIContainer>();
            m_containerFactoryMock.Setup(x => x.CreateContainer()).Returns(containerMock.Object);

            var result = m_configuration.CreateDIContainer();

            m_containerFactoryMock.Verify(x => x.CreateContainer(), Times.Once);
            Assert.That(result, Is.SameAs(containerMock.Object));
        }

        [Test]
        public void WhenCallCreateServiceLocator_ThenShouldCallCreateServiceLocatorOfDIContainerFactory()
        {
            var containerMock = new Mock<IDIContainer>();

            m_configuration.CreateServiceLocator(containerMock.Object);

            m_containerFactoryMock.Verify(x => x.CreateServiceLocator(containerMock.Object), Times.Once);
        }
    }
}
