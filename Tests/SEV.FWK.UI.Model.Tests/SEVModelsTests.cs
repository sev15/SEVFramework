using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SEV.FWK.Service.Tests;
using SEV.Service.Contract;
using SEV.UI.Model;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.FWK.UI.Model.Tests
{
    [TestFixture]
    public class SEVModelsTests : ModelsSysTestBase
    {
        private const int ParentId = 1;
        private const string ParentValue = "Parent";

        [Test]
        public void WhenCallLoadOfListModel_ThenShouldLoadFullCollectionForRequestedEntity()
        {
            var listModel = ServiceLocator.Current.GetInstance<ITestListModel>();

            listModel.Load();

            Assert.That(listModel.IsValid, Is.True);
            Assert.That(listModel.Items.Count, Is.EqualTo(ChildCount + 1));
        }

        [Test]
        public void GivenParentEntityExpressionIsSpecified_WhenCallLoadByIdOfListModel_ThenShouldLoadCollectionOfEntitiesAttachedToParentEntityWithRequestedId()
        {
// ReSharper disable SpecifyACultureInStringConversionExplicitly
            string id = ParentId.ToString();

            var listModel = ServiceLocator.Current.GetInstance<ITestListModel>();

            listModel.Load(id);

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
        public void WhenCallLoadByIdOfSingleModel_ThenShouldLoadEntityWithRequestedId()
        {
            string id = ParentId.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();

            model.Load(id);

            Assert.That(model.IsValid, Is.True);
            Assert.That(model.Id, Is.EqualTo(id));
            Assert.That(model.Value, Is.EqualTo(ParentValue));
        }

        [Test]
        public void GivenParentEntityIsSpecifiedInGetIncludes_WhenCallLoadByIdOfSingleModel_ThenShouldLoadEntityByRequestedIdWithParentEntity()
        {
            string id = ChildCount.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();

            model.Load(id);

            Assert.That(model.IsValid, Is.True);
            Assert.That(model.Id, Is.EqualTo(id));
            Assert.That(model.Value, Is.StringStarting(ChildValue));
            Assert.That(model.Parent, Is.Not.Null);
            Assert.That(model.Parent.Id, Is.EqualTo(ParentId.ToString()));
        }

        [Test]
        public void GivenRelatedEntitiesAreSpecifiedInGetIncludes_WhenCallLoadByIdOfSingleModel_ThenShouldLoadEntityByRequestedIdWithRelatedEntities()
        {
            string id = ParentId.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();

            model.Load(id);

            Assert.That(model.IsValid, Is.True);
            Assert.That(model.Parent, Is.Null);
            Assert.That(model.Children.Count, Is.EqualTo(ChildCount));
        }

        [Test]
        public void GivenNewModel_WhenCallSaveOfEditableModel_ThenShouldCreateNewEntity()
        {
            const string testValue = "Create Test";
            var model = ServiceLocator.Current.GetInstance<ITestModel>();
            model.New();
            model.Value = testValue;

            model.Save();

            string id = (ChildCount + 2).ToString();
            var testModel = ServiceLocator.Current.GetInstance<ITestModel>();
            testModel.Load(id);
            Assert.That(testModel.IsValid, Is.True);
            Assert.That(testModel.Value, Is.EqualTo(testValue));
        }

        [Test]
        public void GivenExistingModel_WhenCallSaveOfEditableModel_ThenShouldUpdateModelEntity()
        {
            var entity = ServiceLocator.Current.GetInstance<ICommandService>().Create(new TestEntity { Value = "new" });
            const string updated = "Update Test";
            string id = entity.Id.ToString();
            var model = ServiceLocator.Current.GetInstance<ITestModel>();
            model.Load(id);
            model.Value = updated;
            var parentModel = ServiceLocator.Current.GetInstance<ITestModel>();
            parentModel.Load(ParentId.ToString());
            model.Parent = parentModel;

            model.Save();

            var testModel = ServiceLocator.Current.GetInstance<ITestModel>();
            testModel.Load(id);
            Assert.That(testModel.IsValid, Is.True);
            Assert.That(testModel.Value, Is.EqualTo(updated));
            Assert.That(testModel.Parent, Is.Not.Null);
            Assert.That(testModel.Parent.Id, Is.EqualTo(ParentId.ToString()));
        }

        [Test]
        public void WhenCallDeleteOfEditableModel_ThenShouldDeleteModelEntity()
        {
            string id = (ChildCount - 1).ToString();
// ReSharper restore SpecifyACultureInStringConversionExplicitly
            var model = ServiceLocator.Current.GetInstance<ITestModel>();
            model.Load(id);
            Assert.That(model.IsValid, Is.True);

            model.Delete();

            var testModel = ServiceLocator.Current.GetInstance<ITestModel>();
            testModel.Load(id);
            Assert.That(testModel.IsValid, Is.False);
        }

        [Test]
        public void GivenModelEntityIsAggregateRoot_WhenCreateModelEntity_ThenShouldCreateChildModelEntities()
        {
            const int count = 3;
            const string testValue = "New Test Parent";
            var parentModel = ServiceLocator.Current.GetInstance<ITestModel>();
            parentModel.New();
            parentModel.Value = testValue;
            Enumerable.Range(1, count).Select(x =>
            {
                var model = ServiceLocator.Current.GetInstance<ITestModel>();
                model.New();
                model.Value = "New Child " + x;
                return model;
            }).ToList().ForEach(c => parentModel.Children.Add(c));

            parentModel.Save();

            var testModel = ServiceLocator.Current.GetInstance<ITestModel>();
            testModel.Load(parentModel.Id);
            Assert.That(testModel.IsValid, Is.True);
            Assert.That(testModel.Children.Count, Is.EqualTo(count));
        }

        [Test]
        public void GivenModelEntityIsAggregateRoot_WhenDeleteModel_ThenShouldDeleteChildEntities()
        {
            var parentModel = ServiceLocator.Current.GetInstance<ITestModel>();
            parentModel.Load(ParentId.ToString());

            parentModel.Delete();

            var testModel = ServiceLocator.Current.GetInstance<ITestListModel>();
            testModel.Load();
            Assert.That(testModel.IsValid, Is.True);
            Assert.That(testModel.Items.Any(), Is.False);
        }

        [Test]
        public void GivenModelEntityIsAggregateRootAndChildModelIsRemoved_WhenUpdateModel_ThenShouldDeleteChildEntity()
        {
            var parentModel = ServiceLocator.Current.GetInstance<ITestModel>();
            parentModel.Load(ParentId.ToString());
            int childrenCount = parentModel.Children.Count;
            var childModel = parentModel.Children.First();
            parentModel.Children.Remove(childModel);

            parentModel.Save();

            var testModel = ServiceLocator.Current.GetInstance<ITestModel>();
            testModel.Load(ParentId.ToString());
            Assert.That(testModel.IsValid, Is.True);
            Assert.That(testModel.Children.Count, Is.EqualTo(childrenCount - 1));
        }
    }

    #region Test Models

    public interface ITestModel : IEditableModel
    {
        string Value { get; set; }
        ITestModel Parent { get; set; }
        IList<ITestModel> Children { get; }
    }

    public interface ITestListModel : IListModel<ITestModel>
    {
    }

    public class TestModel : EditableModel<TestEntity>, ITestModel
    {
        public TestModel(IQueryService queryService, ICommandService commandService, IValidationService validationService)
            : base(queryService, commandService, validationService)
        {
        }

        public string Value
        {
            get { return GetValue(x => x.Value); }
            set { SetValue(value); }
        }

        public ITestModel Parent
        {
            get { return GetReference<ITestModel, TestEntity>(); }
            set { SetReference<ITestModel, TestEntity> (value); }
        }

        public IList<ITestModel> Children => GetCollection<ITestModel, TestEntity>();

        protected override List<Expression<Func<TestEntity, object>>> GetIncludes()
        {
            var includes = base.GetIncludes();

            includes.Add(x => x.Parent);
            includes.Add(x => x.Children);

            return includes;
        }
    }

    public class TestListModel : ListModel<ITestModel, TestEntity>, ITestListModel
    {
        public TestListModel(IQueryService queryService, IParentEntityFilterProvider filterProvider)
            : base(queryService, filterProvider)
        {
            ParentEntityExpression = x => x.Parent;
        }
    }

    #endregion
}
