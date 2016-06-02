using LightInject.ServiceLocation;
using NUnit.Framework;

namespace SEV.DI.LightInject.Tests
{
    [TestFixture]
    public class LightInjectContainerFactoryTests
    {
        private IDIContainerFactory m_containerFactory;

        [SetUp]
        public void Init()
        {
            m_containerFactory = new LightInjectContainerFactory();
        }

        [Test]
        public void WhenCallCreateContainer_ThenShouldReturnInstanceOfLightInjectContainer()
        {
            var result = m_containerFactory.CreateContainer();

            Assert.That(result, Is.InstanceOf<LightInjectContainer>());
        }

        [Test]
        public void WhenCallCreateServiceLocator_ThenShouldReturnInstanceOfLightInjectServiceLocator()
        {
            var container = m_containerFactory.CreateContainer();

            var result = m_containerFactory.CreateServiceLocator(container);

            Assert.That(result, Is.InstanceOf<LightInjectServiceLocator>());
        }
    }
}