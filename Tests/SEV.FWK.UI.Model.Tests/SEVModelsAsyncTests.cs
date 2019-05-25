using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SEV.FWK.Service.Tests;
using SEV.Service.Contract;

namespace SEV.FWK.UI.Model.Tests
{
    [TestFixture]
    public class SEVModelsAsyncTests : ModelsSysTestBase
    {
        private const int ParentId = 1;
        private const string ParentValue = "Parent";

        [Test]
        public async Task WhenCallLoadAsyncOfListModel_ThenShouldLoadFullCollectionForRequestedEntity()
        {
            var listModel = ServiceLocator.Current.GetInstance<ITestListModel>();

            await listModel.LoadAsync();

            Assert.That(listModel.IsValid, Is.True);
            Assert.That(listModel.Items.Count, Is.EqualTo(ChildCount + 1));
        }

        [Test]
        public async Task GivenParentEntityExpressionIsSpecified_WhenCallLoadByIdAsyncOfListModel_ThenShouldLoadCollectionOfEntitiesAttachedToParentEntityWithRequestedId()
        {
// ReSharper disable SpecifyACultureInStringConversionExplicitly
            string id = ParentId.ToString();

            var listModel = ServiceLocator.Current.GetInstance<ITestListModel>();

            await listModel.LoadAsync(id);

            Assert.That(listModel.IsValid, Is.True);
            int count = 0;
            foreach (var model in listModel.Items)
            {
                Assert.That(model.Parent, Is.Not.Null);
                Assert.That(model.Parent.Id, Is.EqualTo(id));
                count++;
            }
            Assert.That(count, Is.EqualTo(ChildCount));
        }

        [Test]
        public async Task WhenCallLoadByIdAsyncOfSingleModel_ThenShouldLoadEntityWithRequestedId()
        {
            string id = ParentId.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();

            await model.LoadAsync(id);

            Assert.That(model.IsValid, Is.True);
            Assert.That(model.Id, Is.EqualTo(id));
            Assert.That(model.Value, Is.EqualTo(ParentValue));
        }

        [Test]
        public async Task GivenParentEntityIsSpecifiedInGetIncludes_WhenCallLoadByIdAsyncOfSingleModel_ThenShouldLoadEntityByRequestedIdWithParentEntity()
        {
            string id = ChildCount.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();

            await model.LoadAsync(id);

            Assert.That(model.IsValid, Is.True);
            Assert.That(model.Id, Is.EqualTo(id));
            Assert.That(model.Value, Is.StringStarting(ChildValue));
            Assert.That(model.Parent, Is.Not.Null);
            Assert.That(model.Parent.Id, Is.EqualTo(ParentId.ToString()));
        }

        [Test]
        public async Task GivenRelatedEntitiesAreSpecifiedInGetIncludes_WhenCallLoadByIdAsyncOfSingleModel_ThenShouldLoadEntityByRequestedIdWithRelatedEntities()
        {
            string id = ParentId.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();

            await model.LoadAsync(id);

            Assert.That(model.IsValid, Is.True);
            Assert.That(model.Parent, Is.Null);
            Assert.That(model.Children.Count, Is.EqualTo(ChildCount));
        }

        [Test]
        public async Task GivenNewModel_WhenCallSaveAsyncOfEditableModel_ThenShouldCreateNewEntity()
        {
            const string testValue = "Create Test";
            var model = ServiceLocator.Current.GetInstance<ITestModel>();
            model.New();
            model.Value = testValue;

            await model.SaveAsync();

            string id = (ChildCount + 2).ToString();
            var newEntity = await ServiceLocator.Current.GetInstance<IQueryService>().FindByIdAsync<TestEntity>(id);
            Assert.That(newEntity, Is.Not.Null);
            Assert.That(newEntity.Value, Is.EqualTo(testValue));
        }

        [Test]
        public async Task GivenExistingModel_WhenCallSaveAsyncOfEditableModel_ThenShouldUpdateModelEntity()
        {
            var entity = await ServiceLocator.Current.GetInstance<ICommandService>()
                                                     .CreateAsync(new TestEntity { Value = "new" });
            const string updated = "Update Test";
            string id = entity.Id.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();
            await model.LoadAsync(id);
            model.Value = updated;
            var parentModel = ServiceLocator.Current.GetInstance<ITestModel>();
            await parentModel.LoadAsync(ParentId.ToString());
            model.Parent = parentModel;

            await model.SaveAsync();

            var updatedEntity = await ServiceLocator.Current.GetInstance<IQueryService>()
                                                            .FindByIdAsync<TestEntity>(id, x => x.Parent);
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.Value, Is.EqualTo(updated));
            Assert.That(updatedEntity.Parent, Is.Not.Null);
            Assert.That(updatedEntity.Parent.Id, Is.EqualTo(ParentId));
        }

        [Test]
        public async Task WhenCallDeleteAsyncOfEditableModel_ThenShouldDeleteModelEntity()
        {
            string id = (ChildCount - 1).ToString();
// ReSharper restore SpecifyACultureInStringConversionExplicitly
            var model = ServiceLocator.Current.GetInstance<ITestModel>();
            await model.LoadAsync(id);
            Assert.That(model.IsValid, Is.True);

            await model.DeleteAsync();

            var deletedEntity = await ServiceLocator.Current.GetInstance<IQueryService>().FindByIdAsync<TestEntity>(id);
            Assert.That(deletedEntity, Is.Null);
        }
    }
}
