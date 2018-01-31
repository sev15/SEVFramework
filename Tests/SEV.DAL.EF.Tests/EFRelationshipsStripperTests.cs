using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Services.Data;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFRelationshipsStripperTests
    {
        private Mock<IRelatedEntitiesStateAdjuster> m_adjusterMock;
        private IRelationshipsStripper<Entity> m_stripper;

        [SetUp]
        public void Init()
        {
            m_adjusterMock = new Mock<IRelatedEntitiesStateAdjuster>();

            m_stripper = new EFRelationshipsStripper<Entity>(m_adjusterMock.Object);
        }

        [Test]
        public void WhenCallStrip_ThenShouldCallAttachRelatedEntitiesOfRelatedEntitiesStateAdjuster()
        {
            var entity = new Mock<Entity>().Object;

            m_stripper.Strip(entity);

            m_adjusterMock.Verify(x => x.AttachRelatedEntities(entity), Times.Once);
        }
    }
}
