using Moq;
using NUnit.Framework;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Logic.Tests
{
    [TestFixture]
    public class DomainEventArgsTests
    {
        private const DomainEvent TestEvent = DomainEvent.Create;

        private Entity m_entity;
        private IUnitOfWork m_unitOfWork;
        private DomainEventArgs<Entity> m_eventArgs;
        
        [SetUp]
        public void Init()
        {
            m_entity = new Mock<Entity>().Object;
            m_unitOfWork = new Mock<IUnitOfWork>().Object;

            m_eventArgs = new DomainEventArgs<Entity>(m_entity, TestEvent, m_unitOfWork);
        }

        [Test]
        public void WhenCreateBusinessRule_ThenShouldInitializeEntityProperty()
        {
            var result = m_eventArgs.Entity;

            Assert.That(result, Is.EqualTo(m_entity));
        }

        [Test]
        public void WhenCreateBusinessRule_ThenShouldInitializeEventProperty()
        {
            var result = m_eventArgs.Event;

            Assert.That(result, Is.EqualTo(TestEvent));
        }

        [Test]
        public void WhenCreateBusinessRule_ThenShouldInitializeUnitOfWorkProperty()
        {
            var result = m_eventArgs.UnitOfWork;

            Assert.That(result, Is.EqualTo(m_unitOfWork));
        }
    }
}
