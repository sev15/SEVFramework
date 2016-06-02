using Moq;
using NUnit.Framework;

namespace SEV.Domain.Model.Tests
{
    [TestFixture]
    public class EntityTests
    {
        private Entity m_entity;

        [SetUp]
        public void Init()
        {
            m_entity = CreateEntity(1);
        }

        [Test]
        public void WhenCallGetEntityId_ThenShouldReturnIdValueTransformedInString()
        {
            const int id = 123;
            m_entity.Id = id;

            var result = m_entity.EntityId;

            Assert.That(result, Is.EqualTo(id.ToString("F0")));
        }

        [Test]
        public void GivenTwoReferencesOnTheSameEntity_WhenCallEquals_ThenShouldReturnTrue()
        {
            var entity = m_entity;

            var result = m_entity.Equals(entity);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenTwoEntitiesWithTheSameIdValue_WhenCallEquals_ThenShouldReturnTrue()
        {
            const int id = 111;
            m_entity.Id = id;
            var entity = CreateEntity(id);

            var result = m_entity.Equals(entity);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenTwoEntitiesWithDifferentIdValue_WhenCallEquals_ThenShouldReturnFalse()
        {
            var entity = CreateEntity(2);

            var result = m_entity.Equals(entity);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenTwoEntitiesWithDefaultIdValue_WhenCallEquals_ThenShouldReturnFalse()
        {
            m_entity = CreateEntity();
            var entity = CreateEntity();

            var result = m_entity.Equals(entity);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenTwoReferencesOnTheSameEntity_WhenCallGetHashCode_ThenShouldReturnTheSameHashCode()
        {
            var entity = m_entity;

            var code1 = m_entity.GetHashCode();
            var code2 = entity.GetHashCode();

            Assert.That(code1, Is.EqualTo(code2));
        }

        [Test]
        public void GivenTwoEntitiesWithTheSameIdValue_WhenCallGetHashCode_ThenShouldReturnTheSameHashCode()
        {
            const int id = 111;
            m_entity.Id = id;
            var entity = CreateEntity(id);

            var code1 = m_entity.GetHashCode();
            var code2 = entity.GetHashCode();

            Assert.That(code1, Is.EqualTo(code2));
        }

        [Test]
        public void GivenTwoEntitiesWithDifferentIdValue_WhenCallGetHashCode_ThenShouldReturnDifferentHashCodes()
        {
            var entity = CreateEntity(2);

            var code1 = m_entity.GetHashCode();
            var code2 = entity.GetHashCode();

            Assert.That(code1, Is.Not.EqualTo(code2));
        }

        [Test]
        public void GivenTwoEntitiesWithDefaultIdValue_WhenCallGetHashCode_ThenShouldReturnDifferentHashCodes()
        {
            m_entity = CreateEntity();
            var entity = CreateEntity();

            var code1 = m_entity.GetHashCode();
            var code2 = entity.GetHashCode();

            Assert.That(code1, Is.Not.EqualTo(code2));
        }

        private Entity CreateEntity(int? id = null)
        {
            var entity = new Mock<Entity> { CallBase = true }.Object;
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }

            return entity;
        }
    }
}