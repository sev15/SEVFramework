using LightInject;
using NUnit.Framework;

namespace SEV.DI.LightInject.Tests
{
    [TestFixture]
    public class LightInjectContainerTests
    {
        private IDIContainer m_container;

        [SetUp]
        public void Init()
        {
            m_container = new LightInjectContainer();
        }

        [Test]
        public void WhenCreateLightInjectContainerObject_ThenShouldReturnInstanceOfServiceContainer()
        {
            Assert.That(m_container, Is.InstanceOf<ServiceContainer>());
        }

        [Test]
        public void GivenEnablePropertyInjectionIsTrue_WhenCreateLightInjectContainerObject_ThenShouldEnablePropertyInjection()
        {
            m_container = new LightInjectContainer(true);

            var propertyDependencySelector = ((ServiceContainer)m_container).PropertyDependencySelector;
            Assert.That(propertyDependencySelector, Is.InstanceOf<PropertyDependencySelector>());
        }

        [Test]
        public void GivenEnablePropertyInjectionIsFalse_WhenCreateLightInjectContainerObject_ThenShouldDisablePropertyInjection()
        {
            var propertyDependencySelector = ((ServiceContainer)m_container).PropertyDependencySelector;
            Assert.That(propertyDependencySelector, Is.Not.InstanceOf<PropertyDependencySelector>());
        }

        [Test]
        public void WhenCallRegisterWithOneGenericParam_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register<LightInjectContainerFactory>();

            Assert.That(m_container.GetService(typeof(LightInjectContainerFactory)), Is.Not.Null);
        }

        [Test]
        public void WhenCallRegisterWithTwoGenericParams_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register<IDIContainerFactory, LightInjectContainerFactory>();

            Assert.That(m_container.GetService(typeof(IDIContainerFactory)), Is.InstanceOf<LightInjectContainerFactory>());
        }

        [Test]
        public void WhenCallRegisterWithTwoGenericParamsAndServiceName_ThenShouldRegisterProvidedTypeInIoC()
        {
            const string serviceName = "test";

            m_container.Register<IDIContainerFactory, LightInjectContainerFactory>(serviceName);

            Assert.That(m_container.GetService(typeof(IDIContainerFactory)), Is.InstanceOf<LightInjectContainerFactory>());
        }

        [Test]
        public void WhenCallRegisterWithOneParam_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register(typeof(LightInjectContainerFactory));

            Assert.That(m_container.GetService(typeof(LightInjectContainerFactory)), Is.Not.Null);
        }

        [Test]
        public void WhenCallRegisterWithTwoParams_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register(typeof(IDIContainerFactory), typeof(LightInjectContainerFactory));

            Assert.That(m_container.GetService(typeof(IDIContainerFactory)), Is.InstanceOf<LightInjectContainerFactory>());
        }

        [Test]
        public void WhenCallRegisterInstance_ThenShouldRegisterProvidedObjectInIoC()
        {
            var testObj = new LightInjectContainerFactory();

            m_container.RegisterInstance<IDIContainerFactory>(testObj);

            Assert.That(m_container.GetService(typeof(IDIContainerFactory)), Is.SameAs(testObj));
        }

        [Test]
        public void GivenRequestedTypeIsRegisteredInServiceContainer_WhenCallGetService_ThenShouldReturnInstanceOfRequestedType()
        {
            m_container.Register<IDIContainerFactory, LightInjectContainerFactory>();

            var result = m_container.GetService(typeof(IDIContainerFactory));

            Assert.That(result, Is.InstanceOf<LightInjectContainerFactory>());
        }
    }
}