using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.Domain.Repository.Tests
{
    // TODO : complete the tests.
    [TestFixture]
    public class RepositoryQueryTests
    {
// ReSharper disable InconsistentNaming
        private const int START_ID = 1;
        private const int TEST_COUNT = 10;
        private const string USER_NAME = "Username";
// ReSharper restore InconsistentNaming

        private Mock<IQueryBuilder<TestEntity>> m_queryBuilderMock;
        private Mock<IQueryable<TestEntity>> m_queryableMock;
        private RepositoryQuery<TestEntity> m_query;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_query = new RepositoryQuery<TestEntity>(m_queryBuilderMock.Object);
        }

        private void InitMocks()
        {
            m_queryBuilderMock = new Mock<IQueryBuilder<TestEntity>>();
            m_queryableMock = new Mock<IQueryable<TestEntity>>();
            var queryable = Enumerable.Range(START_ID, TEST_COUNT).Select(x =>
                                                        new TestEntity { Id = x, Name = USER_NAME + x }).AsQueryable();
            m_queryableMock.Setup(x => x.Provider).Returns(queryable.Provider);
            m_queryableMock.Setup(x => x.Expression).Returns(queryable.Expression);
            m_queryableMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            m_queryableMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator);
            m_queryBuilderMock.Setup(x => x.BuildQuery(It.IsAny<Expression<Func<TestEntity, bool>>>(),
                                        It.IsAny<IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>>(),
                                        It.IsAny<int?>(), It.IsAny<int?>()))
                              .Returns(m_queryableMock.Object);
        }

        #endregion

        [Test]
        public void Get_ShouldCallBuildQueryOfQueryBuilder_WithTwoParameters()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.Name == USER_NAME;
            Expression<Func<TestEntity, object>> orderBy = x => x.Id;

            m_query.Filter(filter).OrderBy(1, orderBy).Get();

            m_queryBuilderMock.Verify(x => x.BuildQuery(filter,
                        It.Is<IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>>(y =>
                        y.Count == 1 && y[1].Item1 == orderBy && y[1].Item2 == false), null, null), Times.Once);
        }

        [Test]
        public void Get_ShouldReturnListOfEntities()
        {
            var result = m_query.Get();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(TEST_COUNT));
        }

        [Test]
        public void GetPage_ShouldCallBuildQueryOfQueryBuilder_WithFourParameters()
        {
            const int page = 2;
            const int size = 4;
            Expression<Func<TestEntity, bool>> filter = x => x.Name == USER_NAME;
            Expression<Func<TestEntity, object>> orderBy = x => x.Id;

            m_query.Filter(filter).OrderBy(1, orderBy).GetPage(page, size);

            m_queryBuilderMock.Verify(x => x.BuildQuery(filter,
                It.Is<IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>>(y => y[1].Item1 == orderBy),
                page, size), Times.Once);
        }

        [Test]
        public void GetPage_ShouldReturnListOfEntities()
        {
            const int page = 1;
            const int size = 4;
            Expression<Func<TestEntity, object>> orderBy = x => x.Id;

            var result = m_query.OrderBy(1, orderBy, true).GetPage(page, size);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(size));
        }
    }
}