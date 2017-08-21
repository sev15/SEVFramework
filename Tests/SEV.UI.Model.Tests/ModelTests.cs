using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SEV.UI.Model.Tests
{
    [TestFixture]
    public class ModelTests
    {
        private Mock<IQueryService> m_queryServiceMock;
        private TestModel m_model;

        [SetUp]
        public void Init()
        {
            m_queryServiceMock = new Mock<IQueryService>();

            m_model = new TestModel(m_queryServiceMock.Object);
        }

        [Test]
        public void WhenCreateModelObject_ThenShouldSetQueryService()
        {
            Assert.That(m_model.QueryService, Is.SameAs(m_queryServiceMock.Object));
        }

        [Test]
        public void WhenCreateModelObject_ThenShouldResetIsInitializedProperty()
        {
            Assert.That(m_model.IsInitialized, Is.False);
        }

        [Test]
        public void WhenCallGetIncludes_ThenShouldReturnEmptyList()
        {
            var result = m_model.GetIncludes();

            Assert.That(result.Any(), Is.False);
        }

        private class TestModel : Model<TestEntity>
        {
            public TestModel(IQueryService queryService) : base(queryService)
            {
            }

            public new bool IsInitialized => base.IsInitialized;

            public new IQueryService QueryService => base.QueryService;

            public new List<Expression<Func<TestEntity, object>>> GetIncludes() => base.GetIncludes();

            public override bool IsValid { get { throw new NotImplementedException(); } }
            public override void Load(string id) { throw new NotImplementedException(); }
            public override Task LoadAsync(string id) { throw new NotImplementedException(); }
        }
    }

    public class TestEntity : Entity
    {
        public string Value { get; set; }
        public TestEntity Parent { get; set; }
        public ICollection<TestEntity> Children { get; set; }
    }
}
