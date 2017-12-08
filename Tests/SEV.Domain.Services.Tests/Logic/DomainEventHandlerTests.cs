using System.Collections.Generic;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Logic.Tests
{
    [TestFixture]
    public class DomainEventHandlerTests
    {
        private Entity m_entity;
        private IUnitOfWork m_unitOfWork;
        private DomainEventArgs<Entity> m_eventArgs;
        private Mock<DomainEventHandler<Entity>> m_eventHandlerMock;

        [SetUp]
        public void Init()
        {
            m_entity = new Mock<Entity>().Object;
            m_unitOfWork = new Mock<IUnitOfWork>().Object;
            m_eventArgs = new DomainEventArgs<Entity>(m_entity, DomainEvent.Delete, m_unitOfWork);
            m_eventHandlerMock = new Mock<DomainEventHandler<Entity>> { CallBase = true };
        }

        [Test]
        public void WhenCallHandle_ThenShouldCallInternalCreateBusinessProcessList()
        {
            m_eventHandlerMock.Protected().Setup<IList<BusinessProcess<Entity>>>("CreateBusinessProcessList", m_eventArgs)
                                          .Returns(new BusinessProcess<Entity>[0]);

            m_eventHandlerMock.Object.Handle(m_eventArgs);

            m_eventHandlerMock.Protected().Verify<IList<BusinessProcess<Entity>>>("CreateBusinessProcessList",
                                                                                  Times.Once(), m_eventArgs);
        }

        [Test]
        public void GivenBusinessProcessListCreated_WhenCallHandle_ThenShouldCallExecuteOfCreatedBusinessProcesses()
        {
            var businessProcessMock = new Mock<BusinessProcess<Entity>>(1, m_entity, m_unitOfWork);
            m_eventHandlerMock.Protected().Setup<IList<BusinessProcess<Entity>>>("CreateBusinessProcessList", m_eventArgs)
                                          .Returns(new[] { businessProcessMock.Object });

            m_eventHandlerMock.Object.Handle(m_eventArgs);

            businessProcessMock.Verify(x => x.Execute(), Times.Once);
        }
    }
}
