using Moq;
using NUnit.Framework;

namespace SEV.Domain.Repository.Tests
{
    [TestFixture]
    public class DomainQueryProviderTests
    {
        private Mock<IDomainQueryHandlerFactory> m_queryHandlerFactoryMock;
        private DomainQueryProvider m_queryProvider;

        [SetUp]
        public void Init()
        {
            m_queryHandlerFactoryMock = new Mock<IDomainQueryHandlerFactory>();

            m_queryProvider = new DomainQueryProvider(m_queryHandlerFactoryMock.Object);
        }

        [Test]
        public void WhenCallCreateQuery_ThenShouldReturnInstanceOfDomainQuery()
        {
            var result = m_queryProvider.CreateQuery();

            Assert.That(result, Is.InstanceOf<DomainQuery>());
        }

        [Test]
        public void WhenCallCreateHandler_ThenShouldCallCreateHandlerOfDomainQueryHandlerFactory()
        {
            const string queryName = "test name";

            m_queryProvider.CreateHandler<string>(queryName);

            m_queryHandlerFactoryMock.Verify(x => x.CreateHandler<string>(queryName), Times.Once);
        }
    }
}