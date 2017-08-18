using LightInject;
using LightInject.Web;
using Moq;
using NUnit.Framework;
using System.Web;

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

        // TODO : find how to unit test the EnableWeb, EnableMvc & EnableWebApi methods.
    }
}