﻿using LightInject;
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
        public void GivenRequestedTypeIsRegisteredInServiceContainer_WhenCallGetService_ThenShouldReturnInstanceOfRequestedType()
        {
            ((ServiceContainer)m_container).Register<IPropertyDependencySelector, NullPropertyDependencySelector>();

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