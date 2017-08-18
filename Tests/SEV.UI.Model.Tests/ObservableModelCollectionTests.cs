using System;
using Moq;
using NUnit.Framework;
using SEV.Service.Contract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SEV.UI.Model.Tests
{
    [TestFixture]
    public class ObservableModelCollectionTests
    {
        private Mock<IQueryService> m_queryServiceMock;
        private IList<TestEntity> m_entityCollection;
        private IList<ITestModel> m_modelCollection;
        private IList<ITestModel> m_observableCollection;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_observableCollection =
                        new ObservableModelCollection<ITestModel, TestEntity>(m_entityCollection, m_modelCollection);
        }

        private void InitMocks()
        {
            m_queryServiceMock = new Mock<IQueryService>();

            m_entityCollection = new List<TestEntity> { new TestEntity { Id = 1 } };
            m_modelCollection = m_entityCollection.Select(CreateModel).ToList();
        }

        private ITestModel CreateModel(TestEntity entity)
        {
            ITestModel model = new TestModel(m_queryServiceMock.Object);
            ((SingleModel<TestEntity>)model).SetEntity(entity);

            return model;
        }

        #endregion

        [Test]
        public void WhenCreateObservableModelCollectionObject_ThenShouldReturnInstanceOfObservableCollectionClass()
        {
            Assert.That(m_observableCollection, Is.InstanceOf<ObservableCollection<ITestModel>>());
        }

        [Test]
        public void WhenAddModel_ThenShouldAddModelEntityInUderlyingEntityCollection()
        {
            var entity = new TestEntity { Id = 123 };
            var model = CreateModel(entity);

            m_observableCollection.Add(model);

            Assert.That(m_entityCollection.Contains(entity), Is.True);
        }

        [Test]
        public void GivenModelIsInvalid_WhenAddModel_ThenShouldThrowInvalidOperationException()
        {
            var model = new TestModel(m_queryServiceMock.Object);

            Assert.That(() => m_observableCollection.Add(model),
                Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(Resources.InsertInvalidModelMsg));
        }

        [Test]
        public void WhenRemoveModel_ThenShouldRemoveModelEntityFromUderlyingEntityCollection()
        {
            var entity = new TestEntity { Id = 123 };
            var model = CreateModel(entity);
            m_observableCollection.Add(model);

            m_observableCollection.Remove(model);

            Assert.That(m_entityCollection.Contains(entity), Is.False);
        }

        [Test]
        public void WhenCallClear_ThenShouldClearUderlyingEntityCollection()
        {
            m_observableCollection.Add(CreateModel(new TestEntity { Id = 123 }));
            m_observableCollection.Add(CreateModel(new TestEntity { Id = 456 }));

            m_observableCollection.Clear();

            Assert.That(m_entityCollection.Any(), Is.False);
        }
    }
}