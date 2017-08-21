using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Repository;
using SEV.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.Service.Tests
{
    [TestFixture]
    public class QueryServiceTests
    {
// ReSharper disable InconsistentNaming
        private static readonly string TEST_ID = 123.ToString("F0");
// ReSharper restore InconsistentNaming

        private Mock<IUnitOfWorkFactory> m_unitOfWorkFactoryMock;
        private Mock<IUnitOfWork> m_unitOfWorkMock;
        private Mock<IRepository<Entity>> m_repositoryMock;
        private Mock<IRelationshipManager<Entity>> m_relationshipManagerMock;
        private Mock<IQuery<Entity>> m_queryMock;
        private IQueryService m_service;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_service = new QueryService(m_unitOfWorkFactoryMock.Object);
        }

        private void InitMocks()
        {
            m_repositoryMock = new Mock<IRepository<Entity>>();
            m_repositoryMock.Setup(x => x.Query())
                            .Returns(new RepositoryQuery<Entity>(m_repositoryMock.As<IQueryBuilder<Entity>>().Object));
            m_unitOfWorkMock = new Mock<IUnitOfWork>();
            m_unitOfWorkMock.Setup(x => x.Repository<Entity>()).Returns(m_repositoryMock.Object);
            m_relationshipManagerMock = new Mock<IRelationshipManager<Entity>>();
            m_unitOfWorkMock.Setup(x => x.RelationshipManager<Entity>()).Returns(m_relationshipManagerMock.Object);
            m_unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            m_unitOfWorkFactoryMock.Setup(x => x.Create()).Returns(m_unitOfWorkMock.Object);
            m_queryMock = new Mock<IQuery<Entity>>();
        }

        #endregion

        [Test]
        public void WhenCallRead_ThenShouldCreateUnitOfWork()
        {
            m_service.Read<Entity>();

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public void WhenCallRead_ThenShouldCallRepositoryOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.Read<Entity>();
            m_unitOfWorkMock.Verify(x => x.Repository<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallRead_ThenShouldCallAllOfEntityRepository()
        {
            m_service.Read<Entity>();

            m_repositoryMock.Verify(x => x.All(), Times.Once);
        }

        [Test]
        public void WhenCallRead_ThenShouldReturnEntityCollectionProvidedByAllQuery()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.All()).Returns(entities);

            var result = m_service.Read<Entity>();

            Assert.That(result, Is.EqualTo(entities));
        }

        [Test]
        public void GivenAllQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallRead_ThenShouldCallRelationshipManagerOfUnitOfWorkForSpecifiedEntity()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.All()).Returns(entities);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.Read(includes);

            m_unitOfWorkMock.Verify(x => x.RelationshipManager<Entity>(), Times.Once);
        }

        [Test]
        public void GivenAllQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallRead_ThenShouldCallLoadOfRelationshipManager()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.All()).Returns(entities);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.Read(includes);

            m_relationshipManagerMock.Verify(x => x.Load(entities, includes), Times.Once);
        }

        [Test]
        public void WhenCallRead_ThenShouldCallDisposeOfUnitOfWork()
        {
            m_service.Read<Entity>();

            m_unitOfWorkMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void WhenCallFindById_ThenShouldCreateUnitOfWork()
        {
            m_service.FindById<Entity>(TEST_ID);

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public void WhenCallFindById_ThenShouldCallRepositoryOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.FindById<Entity>(TEST_ID);

            m_unitOfWorkMock.Verify(x => x.Repository<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallFindById_ThenShouldCallGetByIdOfEntityRepository()
        {
            m_service.FindById<Entity>(TEST_ID);

            m_repositoryMock.Verify(x => x.GetById(TEST_ID), Times.Once);
        }

        [Test]
        public void WhenCallFindById_ThenShouldReturnEntityProvidedByEntityRepository()
        {
            var entity = new Mock<Entity>().Object;
            m_repositoryMock.Setup(x => x.GetById(TEST_ID)).Returns(entity);

            var result = m_service.FindById<Entity>(TEST_ID);

            Assert.That(result, Is.EqualTo(entity));
        }

        [Test]
        public void GivenReturnedEntityIsNotNullAndIncludesAreSpecified_WhenCallFindById_ThenShouldCallRelationshipManagerOfUnitOfWorkForSpecifiedEntity()
        {
            var entity = new Mock<Entity>().Object;
            m_repositoryMock.Setup(x => x.GetById(TEST_ID)).Returns(entity);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.FindById(TEST_ID, includes);

            m_unitOfWorkMock.Verify(x => x.RelationshipManager<Entity>(), Times.Once);
        }

        [Test]
        public void GivenReturnedEntityIsNotNullAndIncludesAreSpecified_WhenCallFindById_ThenShouldCallLoadOfRelationshipManager()
        {
            var entity = new Mock<Entity>().Object;
            m_repositoryMock.Setup(x => x.GetById(TEST_ID)).Returns(entity);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.FindById(TEST_ID, includes);

            m_relationshipManagerMock.Verify(x => x.Load(entity, includes), Times.Once);
        }

        [Test]
        public void WhenCallFindById_ThenShouldCallDisposeOfUnitOfWork()
        {
            m_service.FindById<Entity>(TEST_ID);

            m_unitOfWorkMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void WhenCallFindByIdList_ThenShouldCreateUnitOfWork()
        { 
            m_service.FindByIdList<Entity>(new[] { TEST_ID });

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public void WhenCallFindByIdList_ThenShouldCallRepositoryOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.FindByIdList<Entity>(new[] { TEST_ID });

            m_unitOfWorkMock.Verify(x => x.Repository<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallFindByIdList_ThenShouldCallGetByIdListOfEntityRepository()
        {
            m_service.FindByIdList<Entity>(new[] { TEST_ID });

            m_repositoryMock.Verify(x => x.GetByIdList(It.Is<IEnumerable<object>>(list => 
                                                    list.Single().Equals(TEST_ID))), Times.Once);
        }

        [Test]
        public void WhenCallFindByIdList_ThenShouldReturnEntitiesProvidedByGetByIdListQuery()
        {
            var entities = new Entity[0];
            m_repositoryMock.Setup(x => x.GetByIdList(It.IsAny<IList<object>>())).Returns(entities);

            var result = m_service.FindByIdList<Entity>(new[] { TEST_ID });

            Assert.That(result, Is.EqualTo(entities));
        }

        [Test]
        public void GivenGetByIdListQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallFindByIdList_ThenShouldCallRelationshipManagerOfUnitOfWorkForSpecifiedEntity()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.GetByIdList(It.IsAny<IList<object>>())).Returns(entities);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.FindByIdList(new[] { TEST_ID }, includes);

            m_unitOfWorkMock.Verify(x => x.RelationshipManager<Entity>(), Times.Once);
        }

        [Test]
        public void GivenGetByIdListQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallFindByIdList_ThenShouldCallLoadOfRelationshipManager()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.GetByIdList(It.IsAny<IList<object>>())).Returns(entities);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.FindByIdList(new[] { TEST_ID }, includes);

            m_relationshipManagerMock.Verify(x => x.Load(entities, includes), Times.Once);
        }

        [Test]
        public void WhenCallFindByIdList_ThenShouldCallDisposeOfUnitOfWork()
        {
            m_service.FindByIdList<Entity>(new[] { TEST_ID });

            m_unitOfWorkMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void WhenCallFindByQuery_ThenShouldCreateUnitOfWork()
        {
            m_service.FindByQuery(m_queryMock.Object);

            m_unitOfWorkFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public void WhenCallFindByQuery_ThenShouldCallRepositoryOfUnitOfWorkForSpecifiedEntity()
        {
            m_service.FindByQuery(m_queryMock.Object);

            m_unitOfWorkMock.Verify(x => x.Repository<Entity>(), Times.Once);
        }

        [Test]
        public void WhenCallFindByQuery_ThenShouldCallQueryOfEntityRepository()
        {
            m_service.FindByQuery(m_queryMock.Object);

            m_repositoryMock.Verify(x => x.Query(), Times.Once);
        }

        [Test]
        public void GivenFilterOfProvidedQueryIsNotNull_WhenCallFindByQuery_ThenShouldCallBuildQueryOfEntityRepositoryWithProvidedFilter()
        {
            Expression<Func<Entity, bool>> filter = x => x.Id == 0;
            m_queryMock.SetupGet(x => x.Filter).Returns(filter);

            m_service.FindByQuery(m_queryMock.Object);

            m_repositoryMock.As<IQueryBuilder<Entity>>().Verify(x => x.BuildQuery(filter, null, null, null),
                        Times.Once);
        }

        [Test]
        public void GivenOrderingOfProvidedQueryIsNotNull_WhenCallFindByQuery_ThenShouldCallBuildQueryOfEntityRepositoryWithProvidedOrdering()
        {
            var ordering = new Dictionary<int, Tuple<Expression<Func<Entity, object>>, bool>>
            {
                { 1, new Tuple<Expression<Func<Entity, object>>, bool>(x => x.Id, false) },
            };
            m_queryMock.SetupGet(x => x.Ordering).Returns(ordering);

            m_service.FindByQuery(m_queryMock.Object);

            m_repositoryMock.As<IQueryBuilder<Entity>>().Verify(x => x.BuildQuery(null,
                It.IsAny<IDictionary<int, Tuple<Expression<Func<Entity, object>>, bool>>>(), null, null), Times.Once);
        }

        [Test]
        public void GivenPageCountAndPageSizeOfProvidedQueryAreNotNull_WhenCallFindByQuery_ThenShouldCallBuildQueryOfEntityRepositoryWithProvidedPageCountAndPageSize()
        {
            const int count = 11;
            const int size = 11;
            m_queryMock.SetupGet(x => x.PageCount).Returns(count);
            m_queryMock.SetupGet(x => x.PageSize).Returns(size);

            m_service.FindByQuery(m_queryMock.Object);

            m_repositoryMock.As<IQueryBuilder<Entity>>().Verify(x => x.BuildQuery(null, null, count, size), Times.Once);
        }

        [Test]
        public void WhenCallFindByQuery_ThenShouldReturnEntityCollectionProvidedByBuildQuery()
        {
            var queryBuilderMock = new Mock<IQueryBuilder<Entity>>();
            m_repositoryMock.Setup(x => x.Query()).Returns(new RepositoryQuery<Entity>(queryBuilderMock.Object));
            Expression<Func<Entity, bool>> filter = x => x.Id != 0;
            m_queryMock.SetupGet(x => x.Filter).Returns(filter);
            Entity entity = new Mock<Entity> { CallBase = true }.Object;
            queryBuilderMock.Setup(x => x.BuildQuery(filter, null, null, null))
                            .Returns(new List<Entity> { entity }.AsQueryable());

            var result = m_service.FindByQuery(m_queryMock.Object);

            Assert.That(result, Is.Not.Null);
// ReSharper disable PossibleMultipleEnumeration
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First(), Is.SameAs(entity));
        }

        [Test]
        public void GivenBuildQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallFindByQuery_ThenShouldCallRelationshipManagerOfUnitOfWorkForSpecifiedEntity()
        {
            var queryBuilderMock = new Mock<IQueryBuilder<Entity>>();
            m_repositoryMock.Setup(x => x.Query()).Returns(new RepositoryQuery<Entity>(queryBuilderMock.Object));
            Expression<Func<Entity, bool>> filter = x => x.Id != 0;
            m_queryMock.SetupGet(x => x.Filter).Returns(filter);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };
            m_queryMock.SetupGet(x => x.Includes).Returns(includes);
            Entity entity = new Mock<Entity> { CallBase = true }.Object;
            queryBuilderMock.Setup(x => x.BuildQuery(filter, null, null, null))
                            .Returns(new List<Entity> { entity }.AsQueryable());

            m_service.FindByQuery(m_queryMock.Object);

            m_unitOfWorkMock.Verify(x => x.RelationshipManager<Entity>(), Times.Once);
        }

        [Test]
        public void GivenBuildQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallFindByQuery_ThenShouldCallLoadOfRelationshipManager()
        {
            var queryBuilderMock = new Mock<IQueryBuilder<Entity>>();
            m_repositoryMock.Setup(x => x.Query()).Returns(new RepositoryQuery<Entity>(queryBuilderMock.Object));
            Expression<Func<Entity, bool>> filter = x => x.Id != 0;
            m_queryMock.SetupGet(x => x.Filter).Returns(filter);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };
            m_queryMock.SetupGet(x => x.Includes).Returns(includes);
            Entity entity = new Mock<Entity> { CallBase = true }.Object;
            queryBuilderMock.Setup(x => x.BuildQuery(filter, null, null, null))
                            .Returns(new List<Entity> { entity }.AsQueryable());

            m_service.FindByQuery(m_queryMock.Object);

            m_relationshipManagerMock.Verify(x => x.Load(It.Is<IEnumerable<Entity>>(y => y.Single().Equals(entity)),
                                                            includes), Times.Once);
        }

        [Test]
        public void WhenCallFindByQuery_ThenShouldCallDisposeOfUnitOfWork()
        {
            m_service.FindByQuery(m_queryMock.Object);

            m_unitOfWorkMock.Verify(x => x.Dispose(), Times.Once);
        }

        // INFO : the unit tests for async methods are almost the same as for corresponding sync methods

        [Test]
        public void WhenCallReadAsync_ThenShouldCallAllAsyncOfEntityRepository()
        {
            m_service.ReadAsync<Entity>();

            m_repositoryMock.Verify(x => x.AllAsync(), Times.Once);
        }

        [Test]
        public async void WhenCallReadAsync_ThenShouldReturnEntityCollectionProvidedByAllAsyncQuery()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.AllAsync()).ReturnsAsync(entities);

            var result = await m_service.ReadAsync<Entity>();

            Assert.That(result, Is.EqualTo(entities));
        }

        [Test]
        public void GivenAllQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallReadAsync_ThenShouldCallLoadOfRelationshipManager()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.AllAsync()).ReturnsAsync(entities);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.ReadAsync(includes);

            m_relationshipManagerMock.Verify(x => x.Load(entities, includes), Times.Once);
        }

        [Test]
        public void WhenCallFindByIdAsync_ThenShouldCallGetByIdAsyncOfEntityRepository()
        {
            m_service.FindByIdAsync<Entity>(TEST_ID);

            m_repositoryMock.Verify(x => x.GetByIdAsync(TEST_ID), Times.Once);
        }

        [Test]
        public async void WhenCallFindByIdAsync_ThenShouldReturnEntityProvidedByEntityRepository()
        {
            var entity = new Mock<Entity>().Object;
            m_repositoryMock.Setup(x => x.GetByIdAsync(TEST_ID)).ReturnsAsync(entity);

            var result = await m_service.FindByIdAsync<Entity>(TEST_ID);

            Assert.That(result, Is.EqualTo(entity));
        }

        [Test]
        public void GivenReturnedEntityIsNotNullAndIncludesAreSpecified_WhenCallFindByIdAsync_ThenShouldCallLoadOfRelationshipManager()
        {
            var entity = new Mock<Entity>().Object;
            m_repositoryMock.Setup(x => x.GetByIdAsync(TEST_ID)).ReturnsAsync(entity);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.FindByIdAsync(TEST_ID, includes);

            m_relationshipManagerMock.Verify(x => x.Load(entity, includes), Times.Once);
        }

        [Test]
        public void WhenCallFindByIdListAsync_ThenShouldCallGetByIdListAsyncOfEntityRepository()
        {
            m_service.FindByIdListAsync<Entity>(new[] { TEST_ID });

            m_repositoryMock.Verify(x => x.GetByIdListAsync(It.Is<IEnumerable<object>>(list =>
                                                    list.Single().Equals(TEST_ID))), Times.Once);
        }

        [Test]
        public async void WhenCallFindByIdListAsync_ThenShouldReturnEntitiesProvidedByGetByIdListAsyncQuery()
        {
            var entities = new Entity[0];
            m_repositoryMock.Setup(x => x.GetByIdListAsync(It.IsAny<IList<object>>())).ReturnsAsync(entities);

            var result = await m_service.FindByIdListAsync<Entity>(new[] { TEST_ID });

            Assert.That(result, Is.EqualTo(entities));
        }

        [Test]
        public void GivenGetByIdListQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallFindByIdListAsync_ThenShouldCallLoadOfRelationshipManager()
        {
            var entities = new[] { new Mock<Entity>().Object };
            m_repositoryMock.Setup(x => x.GetByIdListAsync(It.IsAny<IList<object>>())).ReturnsAsync(entities);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };

            m_service.FindByIdListAsync(new[] { TEST_ID }, includes);

            m_relationshipManagerMock.Verify(x => x.Load(entities, includes), Times.Once);
        }

        [Test]
        public void WhenCallFindByQueryAsync_ThenShouldCallQueryOfEntityRepository()
        {
            m_service.FindByQueryAsync(m_queryMock.Object);

            m_repositoryMock.Verify(x => x.Query(), Times.Once);
        }

        [Test]
        public void GivenFilterOfProvidedQueryIsNotNull_WhenCallFindByQueryAsync_ThenShouldCallBuildQueryOfEntityRepositoryWithProvidedFilter()
        {
            Expression<Func<Entity, bool>> filter = x => x.Id == 0;
            m_queryMock.SetupGet(x => x.Filter).Returns(filter);

            m_service.FindByQueryAsync(m_queryMock.Object);

            m_repositoryMock.As<IQueryBuilder<Entity>>().Verify(x => x.BuildQuery(filter, null, null, null),
                        Times.Once);
        }

        [Test]
        public void GivenOrderingOfProvidedQueryIsNotNull_WhenCallFindByQueryAsync_ThenShouldCallBuildQueryOfEntityRepositoryWithProvidedOrdering()
        {
            var ordering = new Dictionary<int, Tuple<Expression<Func<Entity, object>>, bool>>
            {
                { 1, new Tuple<Expression<Func<Entity, object>>, bool>(x => x.Id, false) },
            };
            m_queryMock.SetupGet(x => x.Ordering).Returns(ordering);

            m_service.FindByQueryAsync(m_queryMock.Object);

            m_repositoryMock.As<IQueryBuilder<Entity>>().Verify(x => x.BuildQuery(null,
                It.IsAny<IDictionary<int, Tuple<Expression<Func<Entity, object>>, bool>>>(), null, null), Times.Once);
        }

        [Test]
        public void GivenPageCountAndPageSizeOfProvidedQueryAreNotNull_WhenCallFindByQueryAsync_ThenShouldCallBuildQueryOfEntityRepositoryWithProvidedPageCountAndPageSize()
        {
            const int count = 11;
            const int size = 11;
            m_queryMock.SetupGet(x => x.PageCount).Returns(count);
            m_queryMock.SetupGet(x => x.PageSize).Returns(size);

            m_service.FindByQueryAsync(m_queryMock.Object);

            m_repositoryMock.As<IQueryBuilder<Entity>>().Verify(x => x.BuildQuery(null, null, count, size), Times.Once);
        }

        [Test]
        public async void WhenCallFindByQueryAsync_ThenShouldReturnEntityCollectionProvidedByBuildQuery()
        {
            var queryBuilderMock = new Mock<IQueryBuilder<Entity>>();
            m_repositoryMock.Setup(x => x.Query()).Returns(new RepositoryQuery<Entity>(queryBuilderMock.Object));
            Expression<Func<Entity, bool>> filter = x => x.Id != 0;
            m_queryMock.SetupGet(x => x.Filter).Returns(filter);
            Entity entity = new Mock<Entity> { CallBase = true }.Object;
            queryBuilderMock.Setup(x => x.BuildQuery(filter, null, null, null))
                            .Returns(new List<Entity> { entity }.AsQueryable());

            var result = await m_service.FindByQueryAsync(m_queryMock.Object);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First(), Is.SameAs(entity));
// ReSharper restore PossibleMultipleEnumeration
        }

        [Test]
        public void GivenBuildQueryReturnsNonEmptyCollectionAndIncludesAreSpecified_WhenCallFindByQueryAsync_ThenShouldCallLoadOfRelationshipManager()
        {
            var queryBuilderMock = new Mock<IQueryBuilder<Entity>>();
            m_repositoryMock.Setup(x => x.Query()).Returns(new RepositoryQuery<Entity>(queryBuilderMock.Object));
            Expression<Func<Entity, bool>> filter = x => x.Id != 0;
            m_queryMock.SetupGet(x => x.Filter).Returns(filter);
            var includes = new Expression<Func<Entity, object>>[] { x => x.Id };
            m_queryMock.SetupGet(x => x.Includes).Returns(includes);
            Entity entity = new Mock<Entity> { CallBase = true }.Object;
            queryBuilderMock.Setup(x => x.BuildQuery(filter, null, null, null))
                            .Returns(new List<Entity> { entity }.AsQueryable());

            m_service.FindByQueryAsync(m_queryMock.Object);

            m_relationshipManagerMock.Verify(x => x.Load(It.Is<IEnumerable<Entity>>(y => y.Single().Equals(entity)),
                                                            includes), Times.Once);
        }
    }
}