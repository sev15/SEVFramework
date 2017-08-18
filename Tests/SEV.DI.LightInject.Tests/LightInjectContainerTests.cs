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
        public void WhenCallRegisterWithOneGenericParam_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register<NullPropertyDependencySelector>();

            Assert.That(m_container.GetService(typeof(NullPropertyDependencySelector)), Is.Not.Null);
        }

        [Test]
        public void WhenCallRegisterWithTwoGenericParams_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register<IPropertyDependencySelector, NullPropertyDependencySelector>();

            Assert.That(m_container.GetService(typeof(IPropertyDependencySelector)),
                                                                    Is.InstanceOf<NullPropertyDependencySelector>());
        }

        [Test]
        public void WhenCallRegisterWithTwoGenericParamsAndServiceName_ThenShouldRegisterProvidedTypeInIoC()
        {
            const string serviceName = "test";

            m_container.Register<IPropertyDependencySelector, NullPropertyDependencySelector>(serviceName);

            Assert.That(m_container.GetService(typeof(IPropertyDependencySelector)),
                                                                    Is.InstanceOf<NullPropertyDependencySelector>());
        }

        [Test]
        public void WhenCallRegisterWithOneParam_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register(typeof(NullPropertyDependencySelector));

            Assert.That(m_container.GetService(typeof(NullPropertyDependencySelector)), Is.Not.Null);
        }

        [Test]
        public void WhenCallRegisterWithTwoParams_ThenShouldRegisterProvidedTypeInIoC()
        {
            m_container.Register(typeof(IPropertyDependencySelector), typeof(NullPropertyDependencySelector));

            Assert.That(m_container.GetService(typeof(IPropertyDependencySelector)),
                                                                    Is.InstanceOf<NullPropertyDependencySelector>());
        }

        [Test]
        public void WhenCallRegisterInstance_ThenShouldRegisterProvidedObjectInIoC()
        {
            var testObj = new NullPropertyDependencySelector();

            m_container.RegisterInstance<IPropertyDependencySelector>(testObj);

            Assert.That(m_container.GetService(typeof(IPropertyDependencySelector)), Is.SameAs(testObj));
        }

        [Test]
        public void GivenRequestedTypeIsRegisteredInServiceContainer_WhenCallGetService_ThenShouldReturnInstanceOfRequestedType()
        {
            m_container.Register<IPropertyDependencySelector, NullPropertyDependencySelector>();

            var result = m_container.GetService(typeof(IPropertyDependencySelector));

            Assert.That(result, Is.InstanceOf<NullPropertyDependencySelector>());
        }

        [Test]
        public void WhenCallDisablePropertyInjection_ThenShouldAssignNullPropertyDependencySelectorObjectToPropertyDependencySelectorOfServiceContainer()
        {
            m_container.DisablePropertyInjection();

            Assert.That(((ServiceContainer)m_container).PropertyDependencySelector, Is.InstanceOf<NullPropertyDependencySelector>());
        }
    }
}