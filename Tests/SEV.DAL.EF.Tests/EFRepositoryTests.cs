using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SEV.Domain.Services.Data;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFRepositoryTests
    {
        private Mock<IDbContext> m_dbContextMock;
        private Mock<IDbSet<Entity>> m_dbSetMock;
        private IRepository<Entity> m_repository;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_repository = new EFRepository<Entity>(m_dbContextMock.Object);
        }

        private void InitMocks()
        {
            m_dbSetMock = new Mock<IDbSet<Entity>>();
            m_dbSetMock.As<IDbAsyncEnumerable<Entity>>();
            m_dbContextMock = new Mock<IDbContext>();
            m_dbContextMock.Setup(x => x.Set<Entity>()).Returns(m_dbSetMock.Object);
        }

        private IQueryable<Entity> InitDbSetMock(int count)
        {
            var queryable = Enumerable.Range(1, count).Select(x =>
            {
                var entity = new Mock<Entity> { CallBase = true }.Object;
                entity.Id = x;
                return entity;
            }).AsQueryable();
            m_dbSetMock.Setup(x => x.Provider).Returns(queryable.Provider);
            m_dbSetMock.Setup(x => x.Expression).Returns(queryable.Expression);
            m_dbSetMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            m_dbSetMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator);

            return queryable;
        }

        private IQueryable<Entity> InitDbSetAsyncMock(int count)
        {
            var queryable = Enumerable.Range(1, count).Select(x =>
            {
                var entity = new Mock<Entity> { CallBase = true }.Object;
                entity.Id = x;
                return entity;
            }).AsQueryable();
            m_dbSetMock.As<IDbAsyncEnumerable<Entity>>().Setup(x => x.GetAsyncEnumerator())
                       .Returns(new TestDbAsyncEnumerator<Entity>(queryable.GetEnumerator()));
            m_dbSetMock.As<IQueryable<Entity>>().Setup(x => x.Provider)
                       .Returns(new TestDbAsyncQueryProvider<Entity>(queryable.Provider));
            m_dbSetMock.Setup(x => x.Expression).Returns(queryable.Expression);
            m_dbSetMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            m_dbSetMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator);

            return queryable;
        }

        private class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider m_inner;

            internal TestDbAsyncQueryProvider(IQueryProvider inner)
            {
                m_inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestDbAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestDbAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return m_inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return m_inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }
        }

        private class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
        {
            public TestDbAsyncEnumerable(Expression expression) : base(expression)
            {
            }

            public IDbAsyncEnumerator<T> GetAsyncEnumerator()
            {
                return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
            {
                return GetAsyncEnumerator();
            }

            IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider<T>(this);
        }

        private class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> m_inner;

            public TestDbAsyncEnumerator(IEnumerator<T> inner)
            {
                m_inner = inner;
            }

            public void Dispose()
            {
                m_inner.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(m_inner.MoveNext());
            }

            public T Current => m_inner.Current;

            object IDbAsyncEnumerator.Current => Current;
        }

        #endregion

        [Test]
        public void WhenCreateEFRepositoryObject_ThenShouldCallSetOfDbContext()
        {
            m_dbContextMock.Verify(x => x.Set<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallAll_ThenShouldReturnEntityCollectionProvidedByDbSet()
        {
            var queryable = InitDbSetMock(3);

            var result = m_repository.All();

            Assert.That(result, Is.EquivalentTo(queryable));
        }

        [Test]
        public void WhenCallGetById_ThenShouldCallFindOfDbSet()
        {
            const int id = 3;

            m_repository.GetById(id);

            m_dbSetMock.Verify(x => x.Find(id), Times.Once);
        }

        [Test]
        public void GivenSuppliedIdIsStringValue_WhenCallGetById_ThenShouldCallFindOfDbSetWithIdTransformedInIntValue()
        {
            const int id = 2;
            string idStr = id.ToString();

            m_repository.GetById(idStr);

            m_dbSetMock.Verify(x => x.Find(id), Times.Once);
        }

        [Test]
        public void WhenCallGetByIdList_ThenShouldReturnEntityCollectionProvidedByDbSetFilteredByProvidedIdList()
        {
            const int id1 = 1;
            const int id2 = 4;
            InitDbSetMock(5);

            var result = m_repository.GetByIdList(new[] { id1.ToString(), id2.ToString() });

// ReSharper disable PossibleMultipleEnumeration
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.FirstOrDefault(x => x.Id == id1), Is.Not.Null);
            Assert.That(result.FirstOrDefault(x => x.Id == id2), Is.Not.Null);
        }

        [Test]
        public async void WhenCallAllAsync_ThenShouldReturnEntityCollectionProvidedByDbSet()
        {
            var queryable = InitDbSetAsyncMock(4);

            var result = await m_repository.AllAsync();

            Assert.That(result, Is.EquivalentTo(queryable));
        }

        [Test]
        public async void WhenCallGetByIdAsync_ThenShouldCallReturnEntityWithSpecifiedId()
        {
            const int id = 3;
            InitDbSetAsyncMock(3);

            Entity result = await m_repository.GetByIdAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public async void WhenCallGetByIdListAsync_ThenShouldReturnEntityCollectionProvidedByDbSetFilteredByProvidedIdList()
        {
            const int id1 = 2;
            const int id2 = 3;
            InitDbSetAsyncMock(4);

            var result = await m_repository.GetByIdListAsync(new[] { id1.ToString(), id2.ToString() });

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.FirstOrDefault(x => x.Id == id1), Is.Not.Null);
            Assert.That(result.FirstOrDefault(x => x.Id == id2), Is.Not.Null);
// ReSharper restore PossibleMultipleEnumeration
        }

        [Test]
        public void WhenCallQuery_ThenShouldReturnInstanceOfRepositoryQuery()
        {
            var result = m_repository.Query();

            Assert.That(result, Is.InstanceOf<RepositoryQuery<Entity>>());
        }

        [Test]
        public void WhenCallCreateQuery_ThenShouldReturnEntityCollectionProvidedByDbSet()
        {
            var queryable = InitDbSetMock(4);
            var query = m_repository.Query();

            var result = query.Get();

            Assert.That(result, Is.EquivalentTo(queryable));
        }

        [Test]
        public void WhenCallInsert_ThenShouldCallAddOfDbSet()
        {
            var entity = new Mock<Entity>().Object;

            m_repository.Insert(entity);

            m_dbSetMock.Verify(x => x.Add(entity), Times.Once);
        }

        [Test]
        public void WhenCallRemove_ThenShouldCallRemoveOfDbSet()
        {
            var entity = new Mock<Entity>().Object;

            m_repository.Remove(entity);

            m_dbSetMock.Verify(x => x.Remove(entity), Times.Once);
        }

        [Test]
        public void WhenCallUpdate_ThenShouldSetEntityStateToModified()
        {
            var entity = new Mock<Entity>().Object;

            m_repository.Update(entity);

            m_dbContextMock.Verify(x => x.SetEntityState(entity, EntityState.Modified), Times.Once);
        }
    }
}