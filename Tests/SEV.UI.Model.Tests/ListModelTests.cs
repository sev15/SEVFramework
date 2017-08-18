using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.UI.Model.Tests
{
    [TestFixture]
    public class ListModelTests
    {
        private const int TestId = 123;

        private Mock<IQueryService> m_queryServiceMock;
        private Mock<IParentEntityFilterProvider> m_filterProviderMock;
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private TestEntity m_entity;
        private TestListModel m_model;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_model = new TestListModel(m_queryServiceMock.Object);
        }

        private void InitMocks()
        {
            m_queryServiceMock = new Mock<IQueryService>();
            m_entity = new TestEntity { Id = TestId };
            m_filterProviderMock = new Mock<IParentEntityFilterProvider>();
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<ITestModel>())
                                .Returns(() => new TestModel(m_queryServiceMock.Object));
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
        }

        #endregion

        [Test]
        public void WhenCreateListModelObject_ThenShouldReturnInstanceOfModelClass()
        {
            Assert.That(m_model, Is.InstanceOf<Model<TestEntity>>());
        }

        [Test]
        public void WhenCreateListModelObject_ThenModelItemsShouldNotBeInitialized()
        {
            Assert.That(m_model.IsValid, Is.False);
        }

        [Test]
        public void WhenCallSetItems_ThenShouldCallGetInstanceOfServiceLocatorForTestModel()
        {
            m_model.SetItems(new List<TestEntity> { m_entity });

            m_serviceLocatorMock.Verify(x => x.GetInstance<ITestModel>(), Times.Once);
        }

        [Test]
        public void WhenCallSetItems_ThenShouldInitializeModelItems()
        {
            m_model.SetItems(new List<TestEntity>());

            Assert.That(m_model.IsValid, Is.True);
        }

        [Test]
        public void GivenModelItemsAreInitialized_WhenCallGetItems_ThenShouldReturnModelItems()
        {
            m_model.SetItems(new List<TestEntity> { m_entity });

            var result = m_model.Items;

            Assert.That(result, Is.InstanceOf<IList<ITestModel>>());
            Assert.That(((SingleModel<TestEntity>)result.Single()).ToEntity(), Is.SameAs(m_entity));
        }

        [Test]
        public void GivenModelItemsAreNotInitialized_WhenCallGetItems_ThenShouldReturnNull()
        {
            var result = m_model.Items;

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenCallLoad_ThenShouldCallReadOfQueryService()
        {
            m_model.Load();

            m_queryServiceMock.Verify(x => x.Read<TestEntity>(), Times.Once);
        }

        [Test]
        public void WhenCallLoad_ThenShouldCallGetInstanceOfServiceLocatorForTestModel()
        {
            m_queryServiceMock.Setup(x => x.Read<TestEntity>()).Returns(new List<TestEntity> { m_entity });

            m_model.Load();

            m_serviceLocatorMock.Verify(x => x.GetInstance<ITestModel>(), Times.Once);
        }

        [Test]
        public void WhenCallLoad_ThenShouldInitializeModelItems()
        {
            m_queryServiceMock.Setup(x => x.Read<TestEntity>()).Returns(new List<TestEntity> { m_entity });

            m_model.Load();

            Assert.That(m_model.IsValid, Is.True);
            Assert.That(((SingleModel<TestEntity>)m_model.Items.Single()).ToEntity(), Is.SameAs(m_entity));
        }

        [Test]
        public void GivenParentEntityExpressionIsNotInitialized_WhenCallLoadById_ThenShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => m_model.Load("id"));
        }


        [Test]
        public void GivenParentEntityExpressionIsInitialized_WhenCallGetIncludes_ThenShouldReturnListContainingParentEntityExpression()
        {
            m_model = new TestListModel(m_queryServiceMock.Object, true);

            var result = m_model.GetIncludes();

            Assert.That(result.Single(), Is.SameAs(m_model.ParentEntityExpression));
        }

        [Test]
        public void GivenParentEntityExpressionIsInitializedButParentEntityFilterProviderIsNotInitialized_WhenCallLoadById_ThenShouldThrowInvalidOperationException()
        {
            m_model = new TestListModel(m_queryServiceMock.Object, true);

            Assert.Throws<InvalidOperationException>(() => m_model.Load("id"));
        }

        [Test]
        public void GivenParentEntityExpressionAndParentEntityFilterProviderAreInitialized_WhenCallLoadById_ThenShouldCallGetInstanceOfServiceLocatorForIQueryForTestEntity()
        {
            m_model = new TestListModel(m_queryServiceMock.Object, m_filterProviderMock.Object);
            m_serviceLocatorMock.Setup(x => x.GetInstance<IQuery<TestEntity>>())
                                .Returns(new Mock<IQuery<TestEntity>>().Object);

            m_model.Load("id");

            m_serviceLocatorMock.Verify(x => x.GetInstance<IQuery<TestEntity>>(), Times.Once);
        }

        [Test]
        public void GivenParentEntityExpressionAndParentEntityFilterProviderAreInitialized_WhenCallLoadById_ThenShouldCallCreateFilterOfParentEntityFilterProviderForTestEntity()
        {
            m_model = new TestListModel(m_queryServiceMock.Object, m_filterProviderMock.Object);
            m_serviceLocatorMock.Setup(x => x.GetInstance<IQuery<TestEntity>>())
                                .Returns(new Mock<IQuery<TestEntity>>().Object);
            const string id = "id";

            m_model.Load(id);

            m_filterProviderMock.Verify(x => x.CreateFilter(It.Is<Expression<Func<TestEntity, object>>>(y =>
                                                                y.Body.Type == typeof(TestEntity)), id), Times.Once);
        }

        [Test]
        public void GivenParentEntityExpressionAndParentEntityFilterProviderAreInitialized_WhenCallLoadById_ThenShouldCallFindByQueryOfQueryService()
        {
            m_model = new TestListModel(m_queryServiceMock.Object, m_filterProviderMock.Object);
            var queryMock = new Mock<IQuery<TestEntity>>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<IQuery<TestEntity>>()).Returns(queryMock.Object);

            m_model.Load("id");

            m_queryServiceMock.Verify(x => x.FindByQuery(queryMock.Object));
        }

        [Test]
        public void GivenParentEntityExpressionAndParentEntityFilterProviderAreInitializedAndFindByQuerySucceeds_WhenCallLoadById_ThenShouldCallGetInstanceOfServiceLocatorForTestModel()
        {
            m_model = new TestListModel(m_queryServiceMock.Object, m_filterProviderMock.Object);
            var queryMock = new Mock<IQuery<TestEntity>>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<IQuery<TestEntity>>()).Returns(queryMock.Object);
            m_queryServiceMock.Setup(x => x.FindByQuery(queryMock.Object)).Returns(new List<TestEntity> { m_entity });

            m_model.Load("id");

            m_serviceLocatorMock.Verify(x => x.GetInstance<ITestModel>(), Times.Once);
        }

        [Test]
        public void GivenParentEntityExpressionAndParentEntityFilterProviderAreInitialized_WhenCallLoadById_ThenShouldInitializeModelItems()
        {
            m_model = new TestListModel(m_queryServiceMock.Object, m_filterProviderMock.Object);
            var queryMock = new Mock<IQuery<TestEntity>>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<IQuery<TestEntity>>()).Returns(queryMock.Object);
            m_queryServiceMock.Setup(x => x.FindByQuery(queryMock.Object)).Returns(new List<TestEntity> { m_entity });

            m_model.Load("id");

            Assert.That(m_model.IsValid, Is.True);
            Assert.That(((SingleModel<TestEntity>)m_model.Items.Single()).ToEntity(), Is.SameAs(m_entity));
        }

        private class TestListModel : ListModel<ITestModel, TestEntity>, ITestListModel
        {
            public TestListModel(IQueryService queryService, bool setParent = false) : base(queryService)
            {
                if (setParent)
                {
                    base.ParentEntityExpression = x => x.Parent;
                }
            }

            public TestListModel(IQueryService queryService, IParentEntityFilterProvider filterProvider)
                : base(queryService, filterProvider)
            {
                base.ParentEntityExpression = x => x.Parent;
            }

            public new Expression<Func<TestEntity, object>> ParentEntityExpression => base.ParentEntityExpression;

            public new List<Expression<Func<TestEntity, object>>> GetIncludes() => base.GetIncludes();
        }
    }

    public interface ITestListModel : IListModel<ITestModel>
    {
    }
}