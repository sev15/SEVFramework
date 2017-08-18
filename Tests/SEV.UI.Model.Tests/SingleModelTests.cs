using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.UI.Model.Tests
{
    [TestFixture]
    public class SingleModelTests
    {
        private const int TestId = 123;

        private Mock<IQueryService> m_queryServiceMock;
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private TestEntity m_entity;
        private TestModel m_model;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_model = new TestModel(m_queryServiceMock.Object);
        }

        private void InitMocks()
        {
            m_queryServiceMock = new Mock<IQueryService>();
            m_entity = new TestEntity { Id = TestId };
            m_queryServiceMock.Setup(x => x.FindById<TestEntity>(m_entity.EntityId)).Returns(m_entity);
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<ITestModel>())
                                .Returns(() => new TestModel(m_queryServiceMock.Object));
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
        }

        #endregion

        [Test]
        public void WhenCreateSingleModelObject_ThenShouldReturnInstanceOfModelClass()
        {
            Assert.That(m_model, Is.InstanceOf<Model<TestEntity>>());
        }

        [Test]
        public void WhenCreateSingleModelObject_ThenModelEntityShouldNotBeInitialized()
        {
            Assert.That(m_model.IsValid, Is.False);
        }

        [Test]
        public void WhenCallSetEntity_ThenShouldInitializeModelEntity()
        {
            m_model.SetEntity(m_entity);

            Assert.That(m_model.IsValid, Is.True);
        }

        [Test]
        public void GivenModelEntityIsInitialized_WhenCallToEntity_ThenShouldReturnModelEntity()
        {
            m_model.SetEntity(m_entity);

            var result = m_model.ToEntity();

            Assert.That(result, Is.SameAs(m_entity));
        }

        [Test]
        public void GivenModelEntityIsNotInitialized_WhenCallToEntity_ThenShouldReturnNull()
        {
            var result = m_model.ToEntity();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenCallGetId_ThenShouldReturnValueOfEntityIdOfModelEntity()
        {
            m_model.SetEntity(m_entity);

            var result = m_model.Id;

            Assert.That(result, Is.EqualTo(m_entity.EntityId));
        }

        [Test]
        public void WhenCallLoad_ThenShouldCallFindByIdOfQueryServiceWithProvidedId()
        {
            string id = m_entity.EntityId;

            m_model.Load(id);

            m_queryServiceMock.Verify(x => x.FindById<TestEntity>(id), Times.Once);
        }

        [Test]
        public void GivenGetIncludesIsOverriden_WhenCallLoad_ThenShouldCallFindByIdOfQueryServiceWithProvidedIdAndSpecifiedIncludes()
        {
            m_model = new TestModel(m_queryServiceMock.Object, true);
            string id = m_entity.EntityId;

            m_model.Load(id);

            m_queryServiceMock.Verify(x => x.FindById(id,
                                    It.Is<Expression<Func<TestEntity, object>>[]>(y => y.Length == 1)), Times.Once);
        }

        [Test]
        public void GivenEntityReturnedByFindByIdQueryIsNotNull_WhenCallLoad_ThenShouldInitializeModelEntity()
        {
            m_model.Load(m_entity.EntityId);

            Assert.That(m_model.IsValid, Is.True);
            Assert.That(m_model.ToEntity(), Is.SameAs(m_entity));
        }

        [Test]
        public void GivenEntityReturnedByFindByIdQueryIsNull_WhenCallLoad_ThenShouldNotInitializeModelEntity()
        {
            string id = m_entity.EntityId;
            m_queryServiceMock.Setup(x => x.FindById<TestEntity>(id)).Returns((TestEntity)null);

            m_model.Load(id);

            Assert.That(m_model.IsValid, Is.False);
            Assert.That(m_model.ToEntity(), Is.Null);
        }

        [Test]
        public void GivenModelIsValid_WhenCallGetValue_ThenShouldReturnValueFromEntity()
        {
            m_entity.Value = "test value";
            m_model.SetEntity(m_entity);

            var result = m_model.Value;

            Assert.That(result, Is.EqualTo(m_entity.Value));
        }

        [Test]
        public void GivenModelIsValid_WhenCallGetReference_ThenShouldCallGetInstanceOfServiceLocatorForTestModel()
        {
            m_entity.Parent = new TestEntity();
            m_model.SetEntity(m_entity);

            var result = m_model.Parent;

            m_serviceLocatorMock.Verify(x => x.GetInstance<ITestModel>(), Times.Once);
            Assert.That(result, Is.InstanceOf<ITestModel>());
        }

        [Test]
        public void GivenModelIsValid_WhenCallGetReference_ThenShouldReturnReferenceFromEntityTransformedToModel()
        {
            var reference = new TestEntity();
            m_entity.Parent = reference;
            m_model.SetEntity(m_entity);

            var result = m_model.Parent;

            Assert.That(((SingleModel<TestEntity>)result).ToEntity(), Is.SameAs(reference));
        }

        [Test]
        public void GivenModelIsValid_WhenCallGetCollection_ThenShouldCallGetInstanceOfServiceLocatorForTestModel()
        {
            var reference1 = new TestEntity();
            var reference2 = new TestEntity();
            m_entity.Children = new Collection<TestEntity> { reference1, reference2 };
            m_model.SetEntity(m_entity);

            var result = m_model.Children;

            m_serviceLocatorMock.Verify(x => x.GetInstance<ITestModel>(), Times.Exactly(2));
            Assert.That(result, Is.InstanceOf<IList<ITestModel>>());
        }

        [Test]
        public void GivenModelIsValid_WhenCallGetCollection_ThenShouldReturnObservableCollectionOfEntitiesTransformedToModels()
        {
            var entity1 = new TestEntity { Value = "Entity1" };
            var entity2 = new TestEntity { Value = "Entity2" };
            m_entity.Children = new List<TestEntity> { entity1, entity2 };
            m_model.SetEntity(m_entity);

            var result = m_model.Children;

            Assert.That(result, Is.InstanceOf<ObservableCollection<ITestModel>>());
            ITestModel model1 = result.First(x => x.Value == entity1.Value);
            Assert.That(((SingleModel<TestEntity>)model1).ToEntity(), Is.SameAs(entity1));
            ITestModel model2 = result.First(x => x.Value == entity2.Value);
            Assert.That(((SingleModel<TestEntity>)model2).ToEntity(), Is.SameAs(entity2));
        }
    }

    public interface ITestModel : ISingleModel
    {
        string Value { get; }
        ITestModel Parent { get; }
        IList<ITestModel> Children { get; }
    }

    internal class TestModel : SingleModel<TestEntity>, ITestModel
    {
        private readonly bool m_loadParent;

        public TestModel(IQueryService queryService, bool loadParent = false) : base(queryService)
        {
            m_loadParent = loadParent;
        }

        public string Value => GetValue(x => x.Value);

        public ITestModel Parent => GetReference<ITestModel, TestEntity>();

        public IList<ITestModel> Children => GetCollection<ITestModel, TestEntity>();

        protected override List<Expression<Func<TestEntity, object>>> GetIncludes()
        {
            var includes = base.GetIncludes();

            if (m_loadParent)
            {
                includes.Add(x => x.Parent);
            }

            return includes;
        }
    }
}
