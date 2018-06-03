using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Services.Data;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFRelationshipsStripperTests
    {
        private Mock<IEFRelationshipManagerFactory> m_factoryMock;
        private Mock<IEFRelationshipManager<Entity>> m_managerMock;
        private IRelationshipsStripper<Entity> m_stripper;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_stripper = new EFRelationshipsStripper<Entity>(m_factoryMock.Object);
        }

        private void InitMocks()
        {
            m_managerMock = new Mock<IEFRelationshipManager<Entity>>();
            m_factoryMock = new Mock<IEFRelationshipManagerFactory>();
            m_factoryMock.Setup(x => x.CreateRelationshipManager<Entity>(It.IsAny<DomainEvent>()))
                         .Returns(m_managerMock.Object);
        }

        #endregion

        [Test]
        public void WhenCallStrip_ThenShouldCallCreateRelationshipManagerOfEFRelationshipManagerFactory()
        {
            var entity = new Mock<Entity>().Object;
            var domEvent = DomainEvent.None;

            m_stripper.Strip(entity, domEvent);

            m_factoryMock.Verify(x => x.CreateRelationshipManager<Entity>(domEvent), Times.Once);
        }

        [Test]
        public void WhenCallStrip_ThenShouldCallPrepareRelationshipsOfEFRelationshipManager()
        {
            var entity = new Mock<Entity>().Object;
            var domEvent = DomainEvent.None;

            m_stripper.Strip(entity, domEvent);

            m_managerMock.Verify(x => x.PrepareRelationships(entity), Times.Once);
        }

        [Test]
        public void WhenCallUnStrip_ThenShouldCallRestoreRelationshipsOfEFRelationshipManager()
        {
            var entity = new Mock<Entity>().Object;
            m_stripper.Strip(entity, DomainEvent.None);

            m_stripper.UnStrip(entity);

            m_managerMock.Verify(x => x.RestoreRelationships(entity), Times.Once);
        }
    }
}
