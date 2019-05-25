using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;

namespace SEV.DI.Tests
{
    [TestFixture]
    public class ServiceLocatorExtensionsTests
    {
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private IServiceLocator m_serviceLocator;


        [SetUp]
        public void Init()
        {
            m_serviceLocatorMock = new Mock<IServiceLocator>();

            m_serviceLocator = m_serviceLocatorMock.Object;
        }

        [Test]
        public void WhenCallResolve_ThenShouldCallGetInstanceOfServiceLocator()
        {
            m_serviceLocator.Resolve<IDIContainer>();

            m_serviceLocatorMock.Verify(x => x.GetInstance(typeof(IDIContainer)), Times.Once);
         }

        [Test]
        public void GivenServiceKeyIsSpecified_WhenCallResolve_ThenShouldCallGetInstanceOfServiceLocatorWithProvidedKey()
        {
            const string key = "test key";

            m_serviceLocator.Resolve<IDIContainer>(key);

            m_serviceLocatorMock.Verify(x => x.GetInstance(typeof(IDIContainer), key), Times.Once);
        }

        [Test]
        public void GivenServiceIsRegistered_WhenCallResolve_ThenShouldReturnInstanceOfRegisteredService()
        {
            var service = new Mock<IDIContainer>().Object;
            m_serviceLocatorMock.Setup(x => x.GetInstance(typeof(IDIContainer))).Returns(service);

            var result = m_serviceLocator.Resolve<IDIContainer>();

            Assert.That(result, Is.SameAs(service));
        }

        [Test]
        public void GivenServiceIsNotRegistered_WhenCallResolve_ThenShouldReturnNull()
        {
            m_serviceLocatorMock.Setup(x => x.GetInstance(typeof(IDIContainer))).Throws<ActivationException>();

            var result = m_serviceLocator.Resolve<IDIContainer>();

            Assert.That(result, Is.Null);
        }
    }
}
