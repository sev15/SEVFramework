using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using System.Data.Entity;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFUpdateRelationshipManagerTests
    {
        private Mock<IDbContext> m_dbContextMock;
        private Mock<IDbSet<Entity>> m_dbSetMock;
        private IEFRelationshipManager<Entity> m_manager;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_manager = new EFUpdateRelationshipManager<Entity>(m_dbContextMock.Object);
        }

        private void InitMocks()
        {
            m_dbSetMock = new Mock<IDbSet<Entity>>();
            m_dbContextMock = new Mock<IDbContext>();
            m_dbContextMock.Setup(x => x.Set<Entity>()).Returns(m_dbSetMock.Object);
        }

        #endregion

        [Test]
        public void WhenCallArrangeRelationships_ThenShouldCallGetEntityStateOfDbContext()
        {
            var entity = new Mock<Entity>().Object;

            m_manager.PrepareRelationships(entity);

            m_dbContextMock.Verify(x => x.GetEntityState(entity), Times.Once);
        }

        [Test]
        public void GivenProvidedEntityIsNotAttachedToDbContext_WhenCallArrangeRelationships_ThenShouldCallAttachOfDbSet()
        {
            var entity = new Mock<Entity>().Object;
            m_dbContextMock.Setup(x => x.GetEntityState(entity)).Returns(EntityState.Detached);

            m_manager.PrepareRelationships(entity);

            m_dbSetMock.Verify(x => x.Attach(entity), Times.Once);
        }
    }
}
