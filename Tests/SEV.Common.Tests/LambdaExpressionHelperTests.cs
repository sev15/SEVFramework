using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SEV.Common.Tests
{
    [TestFixture]
    public class LambdaExpressionHelperTests
    {
        [Test]
        public void IsCollectionExpression_ShouldReturnFalse_WhenSuppliedLambdaExpressionIsSimplePropertyExpression()
        {
            Expression<Func<string, object>> lambda = x => x.Length;

            var result = LambdaExpressionHelper.IsCollectionExpression(lambda);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCollectionExpression_ShouldReturnTrue_WhenSuppliedLambdaExpressionIsListExpression()
        {
            Expression<Func<string, object>> lambda = x => x.ToList();

            var result = LambdaExpressionHelper.IsCollectionExpression(lambda);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCollectionExpression_ShouldReturnTrue_WhenSuppliedLambdaExpressionIsIListExpression()
        {
            Expression<Func<string, object>> lambda = x => (IList<char>)x.ToList();

            var result = LambdaExpressionHelper.IsCollectionExpression(lambda);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCollectionExpression_ShouldReturnTrue_WhenSuppliedLambdaExpressionIsICollectionExpression()
        {
            Expression<Func<string, object>> lambda = x => (ICollection<char>)x.ToArray();

            var result = LambdaExpressionHelper.IsCollectionExpression(lambda);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCollectionExpression_ShouldReturnTrue_WhenSuppliedLambdaExpressionIsIEnumerableExpression()
        {
            Expression<Func<string, object>> lambda = x => x.AsEnumerable();

            var result = LambdaExpressionHelper.IsCollectionExpression(lambda);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GetPropertyName_ShouldReturnMemberName_WhenBodyOfSuppliedLambdaExpressionIsMemberExpression()
        {
            Expression<Func<Expression, object>> lambda = x => x.Type;

            var result = LambdaExpressionHelper.GetPropertyName(lambda);

            Assert.That(result, Is.EqualTo("Type"));
        }

        [Test]
        public void GetPropertyName_ShouldThrowArgumentException_WhenBodyOfSuppliedLambdaExpressionIsNotMemberExpression()
        {
            Expression<Func<Expression, object>> lambda = x => x.GetType();

            Assert.Throws<ArgumentException>(() => LambdaExpressionHelper.GetPropertyName(lambda));
        }

        [Test]
        public void GetExpressionMethod_ShouldReturnPropertyInfoObject_WhenBodyOfSuppliedLambdaExpressionIsMemberExpression()
        {
            Expression<Func<Expression, object>> lambda = x => x.Type;

            var result = LambdaExpressionHelper.GetExpressionMethod(lambda);

            Assert.That(result, Is.InstanceOf<PropertyInfo>());
            Assert.That(result.Name, Is.EqualTo("Type"));
        }

        [Test]
        public void GetExpressionMethod_ShouldThrowArgumentException_WhenBodyOfSuppliedLambdaExpressionIsNotMemberExpression()
        {
            Expression<Func<Expression, object>> lambda = x => x.GetType();

            Assert.Throws<ArgumentException>(() => LambdaExpressionHelper.GetExpressionMethod(lambda));
        }
    }
}