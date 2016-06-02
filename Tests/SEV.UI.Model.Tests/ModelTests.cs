using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        public void WhenCreateModelObject_ThenShouldNotInitializeParentEntityExpression()
        {
            Assert.That(m_model.ParentEntityExpression, Is.Null);
        }

        [Test]
        public void GivenParentEntityExpressionIsNotInitialized_WhenCallGetDefaultIncludes_ThenShouldReturnEmptyList()
        {
            var result = m_model.GetDefaultIncludes();

            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void GivenParentEntityExpressionIsInitialized_WhenCallGetDefaultIncludes_ThenShouldReturnListContainingParentEntityExpression()
        {
            m_model = new TestModel(m_queryServiceMock.Object, true);

            var result = m_model.GetDefaultIncludes();

            Assert.That(result.Single(), Is.SameAs(m_model.ParentEntityExpression));
        }

        private class TestModel : Model<TestEntity>
        {
            public TestModel(IQueryService queryService, bool setParent = false) : base(queryService)
            {
                if (setParent)
                {
                    base.ParentEntityExpression = x => x.ParentEntity;
                }
            }

            public new bool IsInitialized
            {
                get { return base.IsInitialized; }
            }

            public new Expression<Func<TestEntity, object>> ParentEntityExpression
            {
                get { return base.ParentEntityExpression; }
            }

            public new IQueryService QueryService
            {
                get { return base.QueryService; }
            }

            public new List<Expression<Func<TestEntity, object>>> GetDefaultIncludes()
            {
                return base.GetDefaultIncludes();
            }

            public override bool IsValid { get { throw new NotImplementedException(); } }
            public override void Load(string id) { throw new NotImplementedException(); }
        }
    }

    public class TestEntity : Entity
    {
        public string Value { get; set; }
        public TestEntity ParentEntity { get; set; }
        public ICollection<TestEntity> Children { get; set; }
    }
}
