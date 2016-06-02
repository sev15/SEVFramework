using Moq;
using NUnit.Framework;

namespace SEV.DI.Web.LightInject.Tests
{
    [TestFixture]
    public class DIContainerExtentionsTests
    {
        [Test]
        public void WhenCallRegisterWebConfigurator_ThenShouldCallRegisterOfDIContainerForIDIContainerWebConfigurator()
        {
            var containerMock = new Mock<IDIContainer>();
            IDIContainer container = containerMock.Object;

            container.RegisterWebConfigurator();

            containerMock.Verify(x => x.Register<IDIContainerWebConfigurator, LightInjectWebConfigurator>(), 
                                    Times.Once);
        }
    }
}