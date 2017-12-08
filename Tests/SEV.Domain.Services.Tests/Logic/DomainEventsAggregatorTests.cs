using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Logic.Tests
{
    [TestFixture]
    public class DomainEventsAggregatorTests
    {
        private const DomainEvent TestEvent = DomainEvent.Update;

        private Entity m_entity;
        private IUnitOfWork m_unitOfWork;
        private DomainEventArgs<Entity> m_eventArgs;
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private IDomainEventsAggregator m_eventsAggregator;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_eventsAggregator = new DomainEventsAggregator();
        }

        private void InitMocks()
        {
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
            m_entity = new Mock<Entity>().Object;
            m_unitOfWork = new Mock<IUnitOfWork>().Object;
            m_eventArgs = new DomainEventArgs<Entity>(m_entity, TestEvent, m_unitOfWork);
        }

        #endregion

        [Test]
        public void WhenCallRaiseEvent_ThenShouldCallGetInstanceOfServiceLocatorForDomainEventHandlerForSpecifiedDomainEvent()
        {
            m_eventsAggregator.RaiseEvent(m_eventArgs);

            m_serviceLocatorMock.Verify(x => x.GetInstance<DomainEventHandler<Entity>>(TestEvent.ToString()), Times.Once);
        }

        [Test]
        public void GivenDomainEventHandlerIsFound_WhenCallRaiseEvent_ThenShouldCallHandleOfDomainEventHandler()
        {
            var eventHandlerMock = new Mock<DomainEventHandler<Entity>>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<DomainEventHandler<Entity>>(TestEvent.ToString()))
                                .Returns(eventHandlerMock.Object);

            m_eventsAggregator.RaiseEvent(m_eventArgs);

            eventHandlerMock.Verify(x => x.Handle(m_eventArgs), Times.Once);
        }

        [Test]
        public void GivenActivationExceptionIsThrown_WhenCallRaiseEvent_ThenShouldNotCallAnything()
        {
            m_serviceLocatorMock.Setup(x => x.GetInstance<DomainEventHandler<Entity>>(TestEvent.ToString()))
                                .Throws<ActivationException>();

            Assert.DoesNotThrow(() => m_eventsAggregator.RaiseEvent(m_eventArgs));
        }
    }
}
