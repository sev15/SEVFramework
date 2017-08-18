using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SEV.DAL.EF;
using SEV.DAL.EF.DI;
using SEV.DI;
using SEV.DI.LightInject;
using SEV.FWK.Service.Tests;
using SEV.Service.Contract;
using SEV.Service.DI;
using SEV.UI.Model;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.FWK.UI.Model.Tests
{
    [TestFixture]
    public class SEVModelsTests
    {
        private const int ParentId = 1;
        private const int ChildCount = 7;
        private const string ParentValue = "Parent";
        private const string ChildValue = "Child";

        #region SetUp

        [SetUp]
        public void Init()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(@"..\.."));

            var diConfiguration = DIConfiguration.Create(new LightInjectContainerFactory());
            IDIContainer container = diConfiguration.CreateDIContainer();
            container.DisablePropertyInjection();
            container.RegisterDomainServices();
            container.RegisterAplicationServices();
            container.Register<IDbContext, TestDbContext>();
            container.Register<EntityAssociationsUpdater, TestEntityAssociationsUpdater>(typeof(TestEntity).FullName);
            container.Register<IParentEntityFilterProvider, ParentEntityFilterProvider>();
            container.Register<ITestModel, TestModel>();
            container.Register<ITestListModel, TestListModel>();

            ServiceLocator.SetLocatorProvider(() => diConfiguration.CreateServiceLocator(container));

            InitDatabase();
        }

        private void InitDatabase()
        {
            var context = (TestDbContext)ServiceLocator.Current.GetInstance<IDbContext>();
            context.Database.ExecuteSqlCommand("DELETE FROM TestEntity;");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('TestEntity', RESEED, 0)");
            context.TestEntities.AddOrUpdate(p => p.Value, new TestEntity { Value = ParentValue });
            context.SaveChanges();
            var parentEntity = context.TestEntities.Single();
            var entities = Enumerable.Range(1, ChildCount)
                                     .Select(x => new TestEntity
                                     {
                                         Value = ChildValue + (ChildCount + 1 - x),
                                         Parent = parentEntity
                                     }).ToArray();
            context.TestEntities.AddOrUpdate(p => p.Value, entities);
            context.SaveChanges();
        }

        #endregion

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
            var newEntity = ServiceLocator.Current.GetInstance<IQueryService>().FindById<TestEntity>(id);
            Assert.That(newEntity, Is.Not.Null);
            Assert.That(newEntity.Value, Is.EqualTo(testValue));
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

            var updatedEntity =
                        ServiceLocator.Current.GetInstance<IQueryService>().FindById<TestEntity>(id, x => x.Parent);
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.Value, Is.EqualTo(updated));
            Assert.That(updatedEntity.Parent, Is.Not.Null);
            Assert.That(updatedEntity.Parent.Id, Is.EqualTo(ParentId));
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

            var deletedEntity = ServiceLocator.Current.GetInstance<IQueryService>().FindById<TestEntity>(id);
            Assert.That(deletedEntity, Is.Null);
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
        public TestModel(IQueryService queryService, ICommandService commandService)
            : base(queryService, commandService)
        {
        }

        public string Value
        {
            get
            {
                return GetValue(x => x.Value);
            }
            set
            {
                SetValue(value);
            }
        }

        public ITestModel Parent
        {
            get
            {
                return GetReference<ITestModel, TestEntity>();
            }
            set
            {
                SetReference<ITestModel, TestEntity> (value);
            }
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
