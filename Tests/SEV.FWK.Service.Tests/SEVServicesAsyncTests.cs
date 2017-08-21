using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SEV.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.FWK.Service.Tests
{
    [TestFixture]
    public class SEVServicesAsyncTests : ServicesSysTestBase
    {
        private const int ParentId = 1;

        [Test]
        public async void WhenExecuteReadAsyncQuery_ThenShouldReturnFullCollectionForRequestedEntity()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = await service.ReadAsync<TestEntity>();

            Assert.That(result, Is.InstanceOf<IEnumerable<TestEntity>>());
            Assert.That(result.Count(), Is.EqualTo(ChildCount + 1));
        }

        [Test]
        public async void GivenIncludesAreSpecified_WhenExecuteReadAsyncQuery_ThenShouldReturnFullCollectionForRequestedEntityWithLoadedIncludes()
        {
            Expression<Func<TestEntity, object>> include = x => x.Parent;
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = await service.ReadAsync(include);

            int count = 0;
            foreach (var entity in result.Where(e => e.Id != ParentId))
            {
                Assert.That(entity.Parent, Is.Not.Null);
                Assert.That(entity.Parent.Id, Is.EqualTo(ParentId));
                count++;
            }
            Assert.That(count, Is.EqualTo(ChildCount));
        }

        [Test]
        public async void WhenExecuteFindByIdAsyncQuery_ThenShouldReturnEntityWithRequestedId()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

// ReSharper disable SpecifyACultureInStringConversionExplicitly
            var result = await service.FindByIdAsync<TestEntity>(ParentId.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(ParentId));
        }

        [Test]
        public async void GivenIncludesAreSpecified_WhenExecuteFindByIdAsyncQuery_ThenShouldReturnEntityWithRequestedIdWithLoadedIncludes()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = await service.FindByIdAsync<TestEntity>(ParentId.ToString(), x => x.Parent, x => x.Children);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parent, Is.Null);
            Assert.That(result.Children, Is.Not.Null);
            Assert.That(result.Children.Count, Is.EqualTo(ChildCount));
        }

        [Test]
        public async void WhenExecuteFindByIdListAsyncQuery_ThenShouldReturnCollectionOfEntitiesWithRequestedIdList()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();
            const int count = 3;
            var idList = Enumerable.Range(3, count).Select(x => x.ToString()).ToList();

            var result = await service.FindByIdListAsync<TestEntity>(idList);

            Assert.That(result, Is.InstanceOf<IEnumerable<TestEntity>>());
// ReSharper disable PossibleMultipleEnumeration
            Assert.That(result.Count(), Is.EqualTo(count));
            foreach (var item in idList)
            {
                var id = Int32.Parse(item);
                Assert.That(result.FirstOrDefault(x => x.Id == id), Is.Not.Null);
            }
        }

        [Test]
        public async void GivenIncludesAreSpecified_WhenExecuteFindByIdListAsyncQuery_ThenShouldReturnCollectionOfEntitiesWithRequestedIdListWithLoadedIncludes()
        {
            const int count = 3;
            var idList = Enumerable.Range(3, count).Select(x => x.ToString()).ToList();
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = await service.FindByIdListAsync<TestEntity>(idList, x => x.Parent);

            foreach (var item in idList)
            {
                var id = Int32.Parse(item);
                var entity = result.Single(x => x.Id == id);
                Assert.That(entity, Is.Not.Null);
                Assert.That(entity.Parent, Is.Not.Null);
            }
        }

        [Test]
        public async void GivenQueryFilterIsSpecified_WhenExecuteFindByQueryAsyncQuery_ThenShouldReturnCollectionOfEntitiesSatisfyingQueryFilter()
        {
            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            const string filter = "4";
            query.Filter = entity => entity.Value.Contains(filter);
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = await service.FindByQueryAsync(query);

            Assert.That(result, Is.InstanceOf<IEnumerable<TestEntity>>());
            Assert.That(result.Single().Value, Is.EqualTo(ChildValue + filter));
        }

        [Test]
        public async void GivenQueryFilterAndIncludesAreSpecified_WhenExecuteFindByQueryAsyncQuery_ThenShouldReturnCollectionOfEntitiesSatisfyingQueryFilterWithLoadedIncludes()
        {
            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            const string filter = "4";
            query.Filter = entity => entity.Value.Contains(filter);
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Parent };
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = await service.FindByQueryAsync(query);

            Assert.That(result.Single().Value, Is.StringContaining(filter));
            Assert.That(result.Single().Parent, Is.Not.Null);
        }

        [Test]
        public async void GivenOrderingAndPagingAreSpecified_WhenExecuteFindByQueryAsyncQuery_ThenShouldReturnCollectionOfEntitiesSatisfyingQuerySettings()
        {
            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            const int size = 5;
            query.Ordering = new Dictionary<int, Tuple<Expression<Func<TestEntity, dynamic>>, bool>>
            {
                { 1, new Tuple<Expression<Func<TestEntity, dynamic>>, bool>(x => x.Id, true) },
                { 2, new Tuple<Expression<Func<TestEntity, dynamic>>, bool>(x => x.Value, false) }
            };
            query.PageCount = 1;
            query.PageSize = size;
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = await service.FindByQueryAsync(query);

            Assert.That(result.Count(), Is.EqualTo(size));
            Assert.That(result.First().Id, Is.EqualTo(ChildCount + 1));
// ReSharper restore PossibleMultipleEnumeration
        }

        [Test]
        public async void WhenExecuteCreateAsyncQuery_ThenShouldReturnEntityWithInitializedId()
        {
            const string testValue = "Create Test";
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            var result = await service.CreateAsync(new TestEntity { Value = testValue });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(default(int)));
            Assert.That(result.Value, Is.EqualTo(testValue));
        }

        [Test]
        public async void GivenParentIsSpecified_WhenExecuteCreateAsyncQuery_ThenShouldReturnEntityLinkedToParentEntity()
        {
            const string testValue = "Create Test 2";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var parentEntity = await queryService.FindByIdAsync<TestEntity>(ParentId.ToString());
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            var result = await service.CreateAsync(new TestEntity { Value = testValue, Parent = parentEntity });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parent, Is.Not.Null);
            Assert.That(result.Parent.Id, Is.EqualTo(ParentId));
            Assert.That(result.Value, Is.EqualTo(testValue));
        }

        [Test]
        public async void WhenExecuteDeleteAsyncQuery_ThenShouldDeleteProvidedEntity()
        {
            string id = (ChildCount - 1).ToString();
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var entityToDelete = await queryService.FindByIdAsync<TestEntity>(id);
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            await service.DeleteAsync(entityToDelete);

            var deletedEntity = await queryService.FindByIdAsync<TestEntity>(id);
            Assert.That(deletedEntity, Is.Null);
        }

        [Test]
        public async void WhenExecuteUpdateAsyncQuery_ThenShouldUpdateProvidedEntity()
        {
            string id = (ParentId + 1).ToString();
            const string updated = "Update Test";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var entityToUpdate = await queryService.FindByIdAsync<TestEntity>(id, x => x.Parent);
            entityToUpdate.Value = updated;
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            await service.UpdateAsync(entityToUpdate);

            var updatedEntity = await queryService.FindByIdAsync<TestEntity>(id, x => x.Parent);
            Assert.That(updatedEntity.Value, Is.EqualTo(updated));
            Assert.That(updatedEntity.Parent, Is.Not.Null);
            Assert.That(updatedEntity.Parent.Id, Is.EqualTo(ParentId));
        }
    }
}
