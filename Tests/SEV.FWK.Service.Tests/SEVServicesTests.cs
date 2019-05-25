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
        private const int CategoryId = 1;

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

        [Test]
        public void GivenCategoryIsSpecified_WhenExecuteCreateQuery_ThenShouldAssociateCreatedEntityWithProvidedCategory()
        {
            const string testValue = "Create With Category Test";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var parentEntity = queryService.FindById<TestEntity>(ParentId.ToString());
            var category = queryService.FindById<TestCategory>(CategoryId.ToString());
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            var result = service.Create(new TestEntity { Value = testValue, Parent = parentEntity, Category = category });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parent, Is.Not.Null);
            Assert.That(result.Parent.Id, Is.EqualTo(ParentId));
            Assert.That(result.Value, Is.EqualTo(testValue));
            Assert.That(result.Category, Is.Not.Null);
            Assert.That(result.Category.Id, Is.EqualTo(CategoryId));
        }

        [Test]
        public void GivenEntityIsAssociatedWithCategory_WhenExecuteDeleteQuery_ThenShouldNotDeleteAssociatedCategory()
        {
            const string testValue = "Delete With Category Test";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var category = queryService.FindById<TestCategory>(CategoryId.ToString());
            var service = ServiceLocator.Current.GetInstance<ICommandService>();
            var testEntity = service.Create(new TestEntity { Value = testValue, Category = category });
            var entityToDelete = queryService.FindById<TestEntity>(testEntity.EntityId);

            service.Delete(entityToDelete);

            Assert.That(queryService.FindById<TestEntity>(testEntity.EntityId), Is.Null);
            Assert.That(queryService.FindById<TestCategory>(CategoryId.ToString()), Is.Not.Null);
        }

        [Test]
        public void GivenCategoryIsProvided_WhenExecuteUpdateQuery_ThenShouldAssociateUpdatedEntityWithProvidedCategory()
        {
            string id = (ParentId + 1).ToString();
            const int categoryId = 1;
// ReSharper restore SpecifyACultureInStringConversionExplicitly
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var entityToUpdate = queryService.FindById<TestEntity>(id, x => x.Parent);
            var category = queryService.FindById<TestCategory>(categoryId.ToString());
            entityToUpdate.Category = category;
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(entityToUpdate);

            var updatedEntity = queryService.FindById<TestEntity>(id, x => x.Parent, x => x.Category);
            Assert.That(updatedEntity.Parent, Is.Not.Null);
            Assert.That(updatedEntity.Category, Is.Not.Null);
            Assert.That(updatedEntity.Category.Id, Is.EqualTo(categoryId));
        }

        [Test]
        public void GivenEntityIsAggregateRoot_WhenExecuteCreateQuery_ThenShouldCreateChildEntities()
        {
            const int count = 3;
            const string testValue = "New Test Parent";
            var newParent = new TestEntity { Value = testValue };
            Enumerable.Range(1, count).Select(x => new TestEntity { Value = "New Child " + x, Parent = newParent })
                      .ToList().ForEach(c => newParent.Children.Add(c));
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Create(newParent);

            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            query.Filter = entity => entity.Value.Contains(testValue);
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Children };
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var createdEntity = queryService.FindByQuery(query).SingleOrDefault();
            Assert.That(createdEntity, Is.Not.Null);
            Assert.That(createdEntity.Children.Count, Is.EqualTo(count));
        }

        [Test]
        public void GivenEntityIsAggregateRoot_WhenExecuteDeleteQuery_ThenShouldDeleteChildEntities()
        {
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var parentEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Delete(parentEntity);

            var entities = queryService.Read<TestEntity>();
            Assert.That(entities.Any(), Is.False);
        }

        [Test]
        public void GivenEntityIsAggregateRootAndChildEntityIsRemoved_WhenExecuteUpdateQuery_ThenShouldDeleteChildEntity()
        {
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var parentEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            int childrenCount = parentEntity.Children.Count;
            var childEntity = parentEntity.Children.First();
            parentEntity.Children.Remove(childEntity);
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(parentEntity);

            var updatedEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            Assert.That(updatedEntity.Children.Count, Is.EqualTo(childrenCount - 1));
        }

        [Test]
        public void GivenEntityIsAggregateRootAndChildEntityIsAdded_WhenExecuteUpdateQuery_ThenShouldCreateChildEntity()
        {
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var parentEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            int childrenCount = parentEntity.Children.Count;
            var childEntity = new TestEntity { Value = "test!!!" };
            parentEntity.Children.Add(childEntity);
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(parentEntity);

            var updatedEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            Assert.That(updatedEntity.Children.Count, Is.EqualTo(childrenCount + 1));
        }

        [Test]
        public void GivenEntityIsAggregateRootAndChildEntitiesAreModified_WhenExecuteUpdateQuery_ThenShouldUpdateChildEntities()
        {
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var parentEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            int childrenCount = parentEntity.Children.Count;
            var childEntity = parentEntity.Children.First();
            parentEntity.Children.Remove(childEntity);
            childEntity = new TestEntity { Value = "test!!!" };
            parentEntity.Children.Add(childEntity);
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(parentEntity);

            var updatedEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            Assert.That(updatedEntity.Children.Count, Is.EqualTo(childrenCount));
        }

        [Test]
        public void GivenEntityIsAggregateRootAndChildEntitiesAreAdded_WhenExecuteUpdateQuery_ThenShouldCreateChildEntities()
        {
            const string testValue = "New Test Parent";
            var service = ServiceLocator.Current.GetInstance<ICommandService>();
            service.Create(new TestEntity { Value = testValue });

            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            query.Filter = entity => entity.Value.Contains(testValue);
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Children };
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var createdEntity = queryService.FindByQuery(query).SingleOrDefault();
            Assert.That(createdEntity, Is.Not.Null);
            Assert.That(createdEntity.Children.Any(), Is.False);

            createdEntity.Children.Add(new TestEntity { Value = "New Child 1" });
            createdEntity.Children.Add(new TestEntity { Value = "New Child 2" });

            service.Update(createdEntity);

            var updatedEntity = queryService.FindByQuery(query).SingleOrDefault();
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.Children.Any(), Is.True);
        }

        [Test]
        public void GivenEntityIsAggregateRootAndCategoriesAreSpecified_WhenExecuteCreateQuery_ThenShouldCreateChildEntitiesWithAssociatedCategories()
        {
            const int count = 3;
            const string testValue = "New Test Parent With Category";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var newParent = new TestEntity { Value = testValue };
            Enumerable.Range(1, count).Select(x => new TestEntity { Value = "New Child " + x })
                      .ToList().ForEach(c => newParent.Children.Add(c));
            var categoriesDic = queryService.Read<TestCategory>().ToDictionary(x => x.Id);
            int categoriesCount = categoriesDic.Count;
            newParent.Category = categoriesDic[1];
            newParent.Children.First().Category = categoriesDic[categoriesCount - 1];
            newParent.Children.Last().Category = categoriesDic[categoriesCount];
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Create(newParent);

            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            query.Filter = entity => entity.Value.Contains(testValue);
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Children, x => x.Category };
            var createdEntity = queryService.FindByQuery(query).SingleOrDefault();
            Assert.That(createdEntity, Is.Not.Null);
            Assert.That(createdEntity.Category, Is.Not.Null);
            Assert.That(createdEntity.Category.Id, Is.EqualTo(categoriesDic[1].Id));
            Assert.That(createdEntity.Children.Count, Is.EqualTo(count));
            Assert.That(queryService.Read<TestCategory>().Count(), Is.EqualTo(categoriesCount));

            query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            query.Filter = entity => entity.Parent.Id == createdEntity.Id && entity.Category != null;
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Category };
            var childrenWithCategories = queryService.FindByQuery(query).ToArray();
            Assert.That(childrenWithCategories.Length, Is.EqualTo(2));
            Assert.That(childrenWithCategories.First().Category.Id, Is.EqualTo(categoriesDic[categoriesCount - 1].Id));
            Assert.That(childrenWithCategories.Last().Category.Id, Is.EqualTo(categoriesDic[categoriesCount].Id));
        }

        [Test]
        public void GivenEntityIsAggregateRootAndCategoriesAreAssociated_WhenExecuteDeleteQuery_ThenShouldNotDeleteAssociatedCategories()
        {
            const string testValue = "New Test Parent With Category";
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            int initialEntitiesCount = queryService.Read<TestEntity>().Count();
            var categoriesDic = queryService.Read<TestCategory>().ToDictionary(x => x.Id);
            var newParent = new TestEntity { Value = testValue, Category = categoriesDic[1] };
            Enumerable.Range(1, 3).Select(x => new TestEntity { Value = "New Child " + x, Category = categoriesDic[x] })
                      .ToList().ForEach(c => newParent.Children.Add(c));
            var service = ServiceLocator.Current.GetInstance<ICommandService>();
            var createdEntity = service.Create(newParent);
            var entityToDelete = queryService.FindById<TestEntity>(createdEntity.EntityId);
            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            query.Filter = entity => entity.Parent.Id == createdEntity.Id;
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Category };
            entityToDelete.Children = queryService.FindByQuery(query).ToList();

            service.Delete(entityToDelete);

            Assert.That(queryService.Read<TestEntity>().Count(), Is.EqualTo(initialEntitiesCount));
        }

        [Test]
        public void GivenEntityIsAggregateRootAndAddChildEntitiesAndAssociateCategories_WhenExecuteUpdateQuery_ThenShouldCreateChildEntitiesWithAssociatedCategories()
        {
            var queryService = ServiceLocator.Current.GetInstance<IQueryService>();
            var categoriesDic = queryService.Read<TestCategory>().ToDictionary(x => x.Id);
            int categoriesCount = categoriesDic.Count;
            var entityToUpdate = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Children);
            entityToUpdate.Category = categoriesDic[1];
            entityToUpdate.Children.First().Category = categoriesDic[2];
            entityToUpdate.Children.Last().Category = categoriesDic[2];
            var childEntityToRemove = entityToUpdate.Children.ElementAt(1);
            entityToUpdate.Children.Remove(childEntityToRemove);
            entityToUpdate.Children.Add(new TestEntity { Value = "New Child 1", Category = categoriesDic[3] });
            entityToUpdate.Children.Add(new TestEntity { Value = "New Child 2", Category = categoriesDic[3] });
            var service = ServiceLocator.Current.GetInstance<ICommandService>();

            service.Update(entityToUpdate);

            var updatedEntity = queryService.FindById<TestEntity>(ParentId.ToString(), x => x.Category);
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.Category, Is.Not.Null);
            Assert.That(updatedEntity.Category.Id, Is.EqualTo(categoriesDic[1].Id));
            Assert.That(queryService.Read<TestCategory>().Count(), Is.EqualTo(categoriesCount));

            var query = ServiceLocator.Current.GetInstance<IQuery<TestEntity>>();
            query.Filter = entity => entity.Parent.Id == updatedEntity.Id && entity.Category != null;
            query.Includes = new Expression<Func<TestEntity, object>>[] { x => x.Category };
            var childrenWithCategories = queryService.FindByQuery(query).ToArray();
            Assert.That(childrenWithCategories.Length, Is.EqualTo(4));
            Assert.That(childrenWithCategories.First().Category.Id, Is.EqualTo(categoriesDic[categoriesCount - 1].Id));
            Assert.That(childrenWithCategories.Last().Category.Id, Is.EqualTo(categoriesDic[categoriesCount].Id));
        }
    }

    public class TestEntity : Entity, IAggregateRoot
    {
        public TestEntity()
        {
            Children = new List<TestEntity>();
        }

        public string Value { get; set; }
        [Parent]
        public TestEntity Parent { get; set; }
        public ICollection<TestEntity> Children { get; set; }
        public TestCategory Category { get; set; }
    }

    public class TestCategory : Entity
    {
        public TestCategory()
        {
            Children = new List<TestEntity>();
        }

        public string Name { get; set; }
        public string Comments { get; set; }
        public ICollection<TestEntity> Children { get; set; }
    }

}
