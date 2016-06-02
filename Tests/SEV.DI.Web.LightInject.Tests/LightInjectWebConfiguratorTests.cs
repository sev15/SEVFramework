using LightInject;
using LightInject.Mvc;
using LightInject.Web;
using Moq;
using NUnit.Framework;
using System.Web;
using System.Web.Mvc;

namespace SEV.DI.Web.LightInject.Tests
{
    [TestFixture]
    public class LightInjectWebConfiguratorTests
    {
        private Mock<IDIContainer> m_containerMock;
        private IDIContainerWebConfigurator m_webConfigurator;

        [SetUp]
        public void Init()
        {
            m_containerMock = new Mock<IDIContainer>();
            m_containerMock.As<IServiceContainer>();

            m_webConfigurator = new LightInjectWebConfigurator();
        }

        [Test]
        public void WhenCallSetContainer_ThenShouldInitializeInternalIDIContainerField()
        {
            m_webConfigurator.SetContainer(m_containerMock.Object);

            Assert.DoesNotThrow(() => m_webConfigurator.RegisterForWeb<IHttpModule, LightInjectHttpModule>());
        }

        [Test]
        public void WhenCallRegisterForWeb_ThenShouldCallRegisterOfServiceContainerWithPerScopeLifetime()
        {
            m_webConfigurator.SetContainer(m_containerMock.Object);

            m_webConfigurator.RegisterForWeb<IHttpModule, LightInjectHttpModule>();

            m_containerMock.As<IServiceContainer>().Verify(x =>
                            x.Register<IHttpModule, LightInjectHttpModule>(It.IsAny<PerScopeLifetime>()), Times.Once);
        }

        //[Test]
        //public void WhenCallRegisterForWeb_ByServiceName_ThenShouldCallRegisterByServiceNameOfServiceContainerWithPerScopeLifetime()
        //{
        //    const string serviceName = "ByServiceName";
        //    m_webConfigurator.SetContainer(m_containerMock.Object);

        //    m_webConfigurator.RegisterForWeb<IHttpModule, LightInjectHttpModule>(serviceName);

        //    m_containerMock.As<IServiceContainer>().Verify(x =>
        //        x.Register<IHttpModule, LightInjectHttpModule>(serviceName, It.IsAny<PerScopeLifetime>()), Times.Once);
        //}

        [Test]
        public void WhenCreateDependencyResolver_ThenShouldReturnInstanceOfLightInjectMvcDependencyResolver()
        {
            IDependencyResolver resolver = m_webConfigurator.CreateDependencyResolver();

            Assert.That(resolver, Is.InstanceOf<LightInjectMvcDependencyResolver>());
        }

        // TODO : find how to unit test the EnableWeb & EnableWebApi methods.
    }
}