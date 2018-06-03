using Moq;
using NUnit.Framework;
using SEV.Domain.Model;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFRelationshipManagerFactoryTests
    {
        private IEFRelationshipManagerFactory m_factory;

        [SetUp]
        public void Init()
        {
            m_factory = new EFRelationshipManagerFactory(new Mock<IDbContext>().Object,
                                                         new Mock<IReferenceContainer>().Object);
        }

        [Test]
        public void GivenProvidedDomainEventIsCreate_WhenCallCreateRelationshipManager_ThenShouldReturnInstanceOfEFCreateRelationshipManager()
        {
            var result = m_factory.CreateRelationshipManager<Entity>(DomainEvent.Create);

            Assert.That(result, Is.InstanceOf<EFCreateRelationshipManager<Entity>>());
        }

        [Test]
        public void GivenProvidedDomainEventIsUpdate_WhenCallCreateRelationshipManager_ThenShouldReturnInstanceOfEFUpdateRelationshipManager()
        {
            var result = m_factory.CreateRelationshipManager<Entity>(DomainEvent.Update);

            Assert.That(result, Is.InstanceOf<EFUpdateRelationshipManager<Entity>>());
        }

        [Test]
        public void GivenProvidedDomainEventIsDelete_WhenCallCreateRelationshipManager_ThenShouldReturnInstanceOfEFDeleteRelationshipManager()
        {
            var result = m_factory.CreateRelationshipManager<Entity>(DomainEvent.Delete);

            Assert.That(result, Is.InstanceOf<EFDeleteRelationshipManager<Entity>>());
        }

        [Test]
        public void GivenProvidedDomainEventIsUndefined_WhenCallCreateRelationshipManager_ThenShouldThrowArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => m_factory.CreateRelationshipManager<Entity>(DomainEvent.None));
        }
    }
}
