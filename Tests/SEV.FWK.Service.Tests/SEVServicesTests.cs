using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.FWK.Service.Tests
{
    [TestFixture]
    public class SEVServicesTests : ServicesSysTestBase
    {
        private const int ParentId = 1;

        [Test]
        public void WhenExecuteReadQuery_ThenShouldReturnFullCollectionForRequestedEntity()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = service.Read<TestEntity>();

            Assert.That(result, Is.InstanceOf<IEnumerable<TestEntity>>());
            Assert.That(result.Count(), Is.EqualTo(ChildCount + 1));
        }

        [Test]
        public void GivenIncludesAreSpecified_WhenExecuteReadQuery_ThenShouldReturnFullCollectionForRequestedEntityWithLoadedIncludes()
        {
            Expression<Func<TestEntity, object>> include = x => x.Parent;
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = service.Read(include);

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
        public void WhenExecuteFindByIdQuery_ThenShouldReturnEntityWithRequestedId()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

// ReSharper disable SpecifyACultureInStringConversionExplicitly
            var result = service.FindById<TestEntity>(ParentId.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(ParentId));
        }

        [Test]
        public void GivenIncludesAreSpecified_WhenExecuteFindByIdQuery_ThenShouldReturnEntityWithRequestedIdWithLoadedIncludes()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = service.FindById<TestEntity>(ParentId.ToString(), x => x.Parent, x => x.Children);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parent, Is.Null);
            Assert.That(result.Children, Is.Not.Null);
            Assert.That(result.Children.Count, Is.EqualTo(ChildCount));
        }

        [Test]
        public void WhenExecuteFindByIdListQuery_ThenShouldReturnCollectionOfEntitiesWithRequestedIdList()
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();
            const int count = 3;
            var idList = Enumerable.Range(3, count).Select(x => x.ToString()).ToList();

            var result = service.FindByIdList<TestEntity>(idList);

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
        public void GivenIncludesAreSpecified_WhenExecuteFindByIdListQuery_ThenShouldReturnCollectionOfEntitiesWithRequestedIdListWithLoadedIncludes()
        {
            const int count = 3;
            var idList = Enumerable.Range(3, count).Select(x => x.ToString()).ToList();
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = service.FindByIdList<TestEntity>(idList, x => x.Parent);

            foreach (var item in idList)
            {
                var id = Int32.Parse(item);
                var entity = result.Single(x => x.Id == id);
                Assert.That(entity, Is.Not.Null);
                Assert.That(entity.Parent, Is.Not.Null);
            }
        }

        [Test]
        public void GivenQueryFilterIsSpecified_WhenExecuteFindByQueryQuery_ThenShouldReturnCollectionOfEntitiesSatisfyingQueryFilter()
        {
            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            const string filter = "4";
            query.Filter = entity => entity.Value.Contains(filter);
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = service.FindByQuery(query);

            Assert.That(result, Is.InstanceOf<IEnumerable<TestEntity>>());
            Assert.That(result.Single().Value, Is.EqualTo(ChildValue + filter));
        }

        [Test]
        public void GivenQueryFilterAndIncludesAreSpecified_WhenExecuteFindByQueryQuery_ThenShouldReturnCollectionOfEntitiesSatisfyingQueryFilterWithLoadedIncludes()
        {
            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            const string filter = "4";
            query.Filter = entity => entity.Value.Contains(filter);
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Parent };
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            var result = service.FindByQuery(query);

            Assert.That(result.Single().Value, Is.StringContaining(filter));
            Assert.That(result.Single().Parent, Is.Not.Null);
        }

        [Test]
        public void GivenOrderingAndPagingAreSpecified_WhenExecuteFindByQueryQuery_ThenShouldReturnCollectionOfEntitiesSatisfyingQuerySettings()
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

            var result = service.FindByQuery(query);

            Assert.That(result.Count(), Is.EqualTo(size));
            Assert.That(result.First().Id, Is.EqualTo(ChildCount + 1));
// ReSharper restore PossibleMultipleEnumeration
        }

        [Test]
        public void WhenExecuteCreateQuery_ThenShouldReturnEntityWithInitializedId()
        {
            const string testValue = "Create Test";
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            var result = service.Create(new TestEntity { Value = testValue });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(default(int)));
            Assert.That(result.Value, Is.EqualTo(testValue));
        }

        [Test]
        public void GivenParentIsSpecified_WhenExecuteCreateQuery_ThenShouldReturnEntityLinkedToParentEntity()
        {
            const string testValue = "Create Test 2";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var parentEntity = queryService.FindById<TestEntity>(ParentId.ToString());
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            var result = service.Create(new TestEntity { Value = testValue, Parent = parentEntity });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parent, Is.Not.Null);
            Assert.That(result.Parent.Id, Is.EqualTo(ParentId));
            Assert.That(result.Value, Is.EqualTo(testValue));
        }

        [Test]
        public void WhenExecuteDeleteQuery_ThenShouldDeleteProvidedEntity()
        {
            string id = (ChildCount - 1).ToString();
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var entityToDelete = queryService.FindById<TestEntity>(id);
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Delete(entityToDelete);

            Assert.That(queryService.FindById<TestEntity>(id), Is.Null);
        }

        [Test]
        public void WhenExecuteUpdateQuery_ThenShouldUpdateProvidedEntity()
        {
            string id = (ParentId + 1).ToString();
            const string updated = "Update Test";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var entityToUpdate = queryService.FindById<TestEntity>(id, x => x.Parent);
            entityToUpdate.Value = updated;
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(entityToUpdate);

            var updatedEntity = queryService.FindById<TestEntity>(id, x => x.Parent);
            Assert.That(updatedEntity.Value, Is.EqualTo(updated));
            Assert.That(updatedEntity.Parent, Is.Not.Null);
            Assert.That(updatedEntity.Parent.Id, Is.EqualTo(ParentId));
        }

        [Test]
        public void GivenEntityAssociationIsDeleted_WhenExecuteUpdateQuery_ThenShouldDeleteAssociationOfProvidedEntity()
        {
            string id = (ParentId + 1).ToString();
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var entityToUpdate = queryService.FindById<TestEntity>(id, x => x.Parent);
            entityToUpdate.Parent = null;
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(entityToUpdate);

            var updatedEntity = queryService.FindById<TestEntity>(id, x => x.Parent);
            Assert.That(updatedEntity.Parent, Is.Null);
        }

        [Test]
        public void GivenEntityAssociationIsModified_WhenExecuteUpdateQuery_ThenShouldUpdateAssociationOfProvidedEntity()
        {
            string id = (ParentId + 1).ToString();
            const int newId = ParentId + 2;
            string newParentId = newId.ToString();
// ReSharper restore SpecifyACultureInStringConversionExplicitly
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var entityToUpdate = queryService.FindById<TestEntity>(id, x => x.Parent);
            var newParentEntity = queryService.FindById<TestEntity>(newParentId);
            entityToUpdate.Parent = newParentEntity;
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(entityToUpdate);

            var updatedEntity = queryService.FindById<TestEntity>(id, x => x.Parent);
            Assert.That(updatedEntity.Parent, Is.Not.Null);
            Assert.That(updatedEntity.Parent.Id, Is.EqualTo(newId));
        }
    }

    public class TestEntity : Entity
    {
        public TestEntity()
        {
            Children = new List<TestEntity>();
        }

        public string Value { get; set; }
        public TestEntity Parent { get; set; }
        public ICollection<TestEntity> Children { get; set; }
    }
}
