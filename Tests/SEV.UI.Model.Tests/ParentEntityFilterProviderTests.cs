using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace SEV.UI.Model.Tests
{
    [TestFixture]
    public class ParentEntityFilterProviderTests
    {
        private const int TestId = 123;
        private static readonly string TestIdStr = TestId.ToString();

        private Expression<Func<TestEntity, object>> m_parentExpression;
        private IParentEntityFilterProvider m_filterProvider;

        [SetUp]
        public void Init()
        {
            m_parentExpression = x => x.ParentEntity;

            m_filterProvider = new ParentEntityFilterProvider();
        }

        [Test]
        public void WhenCallCreateFilter_ThenShouldReturnFilterExpressionForGivenEntityType()
        {
            var result = m_filterProvider.CreateFilter(m_parentExpression, TestIdStr);

            Assert.That(result, Is.InstanceOf<Expression<Func<TestEntity, bool>>>());
        }

        [Test]
        public void WhenCallCreateFilter_ThenShouldReturnFilterExpressionForParentEntityIdProperty()
        {
            var result = m_filterProvider.CreateFilter(m_parentExpression, TestIdStr);

            Assert.That(result.Body, Is.InstanceOf<BinaryExpression>());
            var propertyExpr = ((BinaryExpression)result.Body).Left;
            Assert.That(propertyExpr, Is.InstanceOf<MemberExpression>());
            Assert.That(((MemberExpression)propertyExpr).Member.Name, Is.EqualTo("Id"));
            var expectedExprStr = m_parentExpression.Body + "." + "Id";
            Assert.That(propertyExpr.ToString(), Is.EqualTo(expectedExprStr));
        }

        [Test]
        public void WhenCallCreateFilter_ThenShouldReturnFilterExpressionForGivenEntityIdValue()
        {
            var result = m_filterProvider.CreateFilter(m_parentExpression, TestIdStr);

            var idValueExpr = ((BinaryExpression)result.Body).Right;
            Assert.That(idValueExpr, Is.InstanceOf<ConstantExpression>());
            Assert.That(((ConstantExpression)idValueExpr).Value, Is.EqualTo(TestId));
        }
    }
}