using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class ReferenceContainerTests
    {
        private TestEntity m_entity;
        private IReferenceContainer m_container;

        #region SetUp

        [SetUp]
        public void Init()
        {
            m_entity = CreateTestEntity();

            m_container = new ReferenceContainer();
        }

        private TestEntity CreateTestEntity()
        {
            return new TestEntity
            {
                Parent = new TestEntity(),
                Children = new List<TestEntity> { new TestEntity() }
            };
        }

        #endregion

        [Test]
        public void WhenCallAnalyzeReferences_ThenShouldFindEntityReferences()
        {
            m_container.AnalyzeReferences(m_entity);

            Assert.That(m_container.GetRelationships().Length, Is.EqualTo(1));
        }

        [Test]
        public void WhenCallAnalyzeReferences_ThenShouldStripEntityCollections()
        {
            m_container.AnalyzeReferences(m_entity);

            Assert.That(m_entity.Children, Is.Null);
        }

        [Test]
        public void WhenCallAnalyzeReferences_ThenShouldFindChildCollection()
        {
            m_container.AnalyzeReferences(m_entity);

            var result = m_container.GetChildCollections(m_entity);

            Assert.That(result.Any(), Is.True);
        }

        [Test]
        public void WhenCallRestoreReferences_ThenShouldRestoreEntityCollections()
        {
            m_container.AnalyzeReferences(m_entity);

            m_container.RestoreReferences(m_entity);

            Assert.That(m_entity.Children, Is.Not.Null);
        }
    }
}