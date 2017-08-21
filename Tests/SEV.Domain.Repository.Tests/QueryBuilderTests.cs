using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.Domain.Repository.Tests
{
    [TestFixture]
    public class QueryBuilderTests
    {
// ReSharper disable InconsistentNaming
        private const int START_ID = 1;
        private const int TEST_COUNT = 10;
        private const string TEST_NAME = "Name";
// ReSharper restore InconsistentNaming

        private Mock<IQueryable<TestEntity>> m_queryableMock;
        private QueryBuilder<TestEntity> m_queryBuilder;

        #region SetUp

        [SetUp]
        public void Init()
        {
            m_queryBuilder = InitMocks();
        }

        private QueryBuilder<TestEntity> InitMocks()
        {
            m_queryableMock = new Mock<IQueryable<TestEntity>>();
            var queryable = Enumerable.Range(START_ID, TEST_COUNT).Select(x =>
                new TestEntity
                {
                    Id = x,
                    Name = TEST_NAME + x,
                    Amount = 10m * x,
                    CreationDate = DateTime.Today.AddDays(1 - x),
                    ModificationDate = DateTime.Today.AddDays(x - 1),
                    ID = Guid.NewGuid(),
                    Unsupported = (byte)(x * 8)
                }).AsQueryable();
            m_queryableMock.Setup(x => x.Provider).Returns(queryable.Provider);
            m_queryableMock.Setup(x => x.Expression).Returns(queryable.Expression);
            m_queryableMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            m_queryableMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator);
            var queryBuilderMock = new Mock<QueryBuilder<TestEntity>> { CallBase = true };
            queryBuilderMock.Protected().Setup<IQueryable<TestEntity>>("CreateQuery").Returns(m_queryableMock.Object);

            return queryBuilderMock.Object;
        }

        #endregion

        [Test]
        public void BuildQuery_ShouldFilterQueryableCollection_WhenFilterIsNotNull()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.Id == TEST_COUNT;

            var result = m_queryBuilder.BuildQuery(filter);

            var test = result.ToList();
            Assert.That(test.Single().Id, Is.EqualTo(TEST_COUNT));
        }

        [Test]
        public void BuildQuery_ShouldOrderQueryableCollection_WhenOrderByIsNotNull()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 2, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Name, true) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Id, true) },
                    { 4, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.ModificationDate, true) },
                    { 3, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.CreationDate, true) },
                    { 5, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.ID, true) },
                };

            var result = m_queryBuilder.BuildQuery(null, orderBys);

            var test = result.ToList();
            Assert.That(test.First().Id, Is.EqualTo(TEST_COUNT));
        }
        
        [Test]
        public void BuildQuery_ShouldReturnSpecifiedSubCollection_WhenOrderByAndPageAndSizeAreNotNull()
        {
            const int page = 2;
            const int size = 4;
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 0, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Name, true) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Id, false) },
                    { 2, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Amount, true) }
                };

            var result = m_queryBuilder.BuildQuery(null, orderBys, page, size);

            var test = result.ToList();
            Assert.That(test, Is.Not.Null);
            Assert.That(test.Count, Is.EqualTo(size));
            Assert.That(test.First().Id, Is.EqualTo(START_ID + (page - 1) * size));
        }

        [Test]
        public void BuildQuery_ShouldThrowInvalidOperationException_WhenPageAndSizeAreNotNullAndOrderByIsNull()
        {
            Assert.Throws<InvalidOperationException>(() => m_queryBuilder.BuildQuery(null, null, 1, 3));
        }

        [Test]
        public void BuildQuery_ShouldThrowInvalidOperationException_WhenPageIsNotNullAndSizeIsNull()
        {
            Assert.Throws<InvalidOperationException>(() => m_queryBuilder.BuildQuery(null, null, 1));
        }

        [Test]
        public void BuildQuery_ShouldThrowInvalidOperationException_WhenPageIsNullAndSizeIsNotNull()
        {
            Assert.Throws<InvalidOperationException>(() => m_queryBuilder.BuildQuery(null, null, null, 2));
        }

        [Test]
        public void BuildQuery_ShouldThrowArgumentException_WhenOrderByIsNotValid()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.ToString(), true) }
                };

            Assert.Throws<ArgumentException>(() => m_queryBuilder.BuildQuery(null, orderBys));
        }

        [Test]
        public void BuildQuery_ShouldThrowArgumentException_WhenAnotherOrderByIsNotValid()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 0, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Name, true) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.GetType(), true) }
                };

            Assert.Throws<ArgumentException>(() => m_queryBuilder.BuildQuery(null, orderBys));
        }

        [Test]
        public void BuildQuery_ShouldOrderQueryableCollection_WhenOrderByIntValue()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 2, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.CreationDate, false) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Id, true) }
                };

            var result = m_queryBuilder.BuildQuery(null, orderBys);

            var test = result.ToList();
            Assert.That(test.First().Id, Is.EqualTo(test.Select(x => x.Id).OrderByDescending(y => y).First()));
        }

        [Test]
        public void BuildQuery_ShouldOrderQueryableCollection_WhenOrderByGuidValue()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 2, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.CreationDate, false) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.ID, true) }
                };

            var result = m_queryBuilder.BuildQuery(null, orderBys);

            var test = result.ToList();
            Assert.That(test.First().ID, Is.EqualTo(test.Select(x => x.ID).OrderByDescending(y => y).First()));
        }

        [Test]
        public void BuildQuery_ShouldOrderQueryableCollection_WhenOrderByDateTimeValue()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 0, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.CreationDate, true) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Id, false) }
                };

            var result = m_queryBuilder.BuildQuery(null, orderBys);

            var test = result.ToList();
            Assert.That(test.First().CreationDate, Is.EqualTo(DateTime.Today));
        }

        [Test]
        public void BuildQuery_ShouldOrderQueryableCollection_WhenOrderByNullableDateTimeValue()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 0, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.ModificationDate, false) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Id, true) }
                };

            var result = m_queryBuilder.BuildQuery(null, orderBys);

            var test = result.ToList();
            Assert.That(test.First().ModificationDate, Is.EqualTo(DateTime.Today));
        }

        [Test]
        public void BuildQuery_ShouldOrderQueryableCollection_WhenOrderByDecimalValue()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 0, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Amount, true) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Id, false) }
                };

            var result = m_queryBuilder.BuildQuery(null, orderBys);

            var test = result.ToList();
            Assert.That(test.First().Id, Is.EqualTo(TEST_COUNT));
        }

        [Test]
        public void BuildQuery_ShouldThrowArgumentException_WhenOrderByUnsupportedTypeValue()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Unsupported, true) }
                };

            Assert.Throws<ArgumentException>(() => m_queryBuilder.BuildQuery(null, orderBys));
        }

        [Test]
        public void BuildQuery_ShouldThrowArgumentException_WhenAnotherOrderByUnsupportedTypeValue()
        {
            IDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>> orderBys =
                new SortedDictionary<int, Tuple<Expression<Func<TestEntity, object>>, bool>>
                {
                    { 0, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.EntityId, true) },
                    { 1, new Tuple<Expression<Func<TestEntity, object>>, bool>(x => x.Unsupported, false) }
                };

            Assert.Throws<ArgumentException>(() => m_queryBuilder.BuildQuery(null, orderBys));
        }
    }

    public class TestEntity : Entity
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public decimal Amount { get; set; }
        public Guid ID { get; set; }
        public byte Unsupported { get; set; }
    }
}