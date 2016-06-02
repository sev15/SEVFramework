﻿using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;

namespace SEV.UI.Model.Tests
{
    [TestFixture]
    public class EditableModelTests
    {
        private const int TestId = 123;

        private Mock<IQueryService> m_queryServiceMock;
        private Mock<ICommandService> m_commandServiceMock;  
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private TestEntity m_entity;
        private TestModel2 m_model;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_model = new TestModel2(m_queryServiceMock.Object, m_commandServiceMock.Object);
        }

        private void InitMocks()
        {
            m_queryServiceMock = new Mock<IQueryService>();
            m_commandServiceMock = new Mock<ICommandService>();
            m_entity = new TestEntity { Id = TestId };
            //m_queryServiceMock.Setup(x => x.FindById<TestEntity>(m_entity.EntityId)).Returns(m_entity);
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<ITestModel>())
                                .Returns(() => new TestModel(m_queryServiceMock.Object));
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
        }

        #endregion

        [Test]
        public void WhenCreateEditableModelObject_ThenShouldReturnInstanceOfSingleModelClass()
        {
            Assert.That(m_model, Is.InstanceOf<SingleModel<TestEntity>>());
        }

        [Test]
        public void WhenCreateEditableModelObject_ThenShouldSetCommandService()
        {
            Assert.That(m_model.CommandService, Is.SameAs(m_commandServiceMock.Object));
        }

        [Test]
        public void WhenCreateEditableModelObject_ThenShouldResetIsNewProperty()
        {
            Assert.That(m_model.IsNew, Is.False);
        }

        [Test]
        public void WhenCallNew_ThenShouldSetIsNewToTrue()
        {
            m_model.New();

            Assert.That(m_model.IsNew, Is.True);
        }

        [Test]
        public void WhenCallNew_ThenShouldInitializeModelEntity()
        {
            m_model.New();

            Assert.That(m_model.IsValid, Is.True);
            Assert.That(m_model.ToEntity(), Is.Not.Null);
            Assert.That(m_model.ToEntity().Id, Is.EqualTo(default(int)));
        }

        [Test]
        public void GivenModelEntityIsNotInitialized_WhenCallSave_ThenShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => m_model.Save());
        }

        [Test]
        public void GivenModelIsNew_WhenCallSave_ThenShouldCallCreateOfCommandService()
        {
            m_model.New();

            m_model.Save();

            m_commandServiceMock.Verify(x => x.Create(It.Is<TestEntity>(y => y.Id == default(int))), Times.Once);
        }

        [Test]
        public void GivenCreateQuerySucceeds_WhenCallSave_ThenShouldInitializeModelEntityWithOneCreatedByCommandService()
        {
            m_model.New();
            m_commandServiceMock.Setup(x => x.Create(It.Is<TestEntity>(y => y.Id == default(int)))).Returns(m_entity);

            m_model.Save();

            Assert.That(m_model.ToEntity(), Is.SameAs(m_entity));
        }

        [Test]
        public void GivenCreateQuerySucceeds_WhenCallSave_ThenShouldReInitializeModelEntity()
        {
            m_model.New();
            m_commandServiceMock.Setup(x => x.Create(It.Is<TestEntity>(y => y.Id == default(int)))).Returns(m_entity);

            m_model.Save();

            Assert.That(m_model.IsValid, Is.True);
            Assert.That(m_model.IsNew, Is.False);
        }

        [Test]
        public void GivenCreateQueryFails_WhenCallSave_ThenShouldNotReInitializeModelEntity()
        {
            m_model.New();
            m_commandServiceMock.Setup(x => x.Create(It.IsAny<TestEntity>())).Throws<Exception>();

            Assert.Throws<Exception>(() => m_model.Save());

            Assert.That(m_model.IsValid, Is.False);
            Assert.That(m_model.IsNew, Is.True);
        }

        [Test]
        public void GivenModelIsNotNew_WhenCallSave_ThenShouldCallUpdateOfCommandService()
        {
            m_model.SetEntity(m_entity);

            m_model.Save();

            m_commandServiceMock.Verify(x => x.Update(m_entity), Times.Once);
        }

        [Test]
        public void GivenUpdateQuerySucceeds_WhenCallSave_ThenShouldReInitializeModelEntity()
        {
            m_model.SetEntity(m_entity);

            m_model.Save();

            Assert.That(m_model.IsValid, Is.True);
        }

        [Test]
        public void GivenUpdateQueryFails_WhenCallSave_ThenShouldNotReInitializeModelEntity()
        {
            m_model.SetEntity(m_entity);
            m_commandServiceMock.Setup(x => x.Update(m_entity)).Throws<Exception>();

            Assert.Throws<Exception>(() => m_model.Save());

            Assert.That(m_model.IsValid, Is.False);
        }

        [Test]
        public void GivenModelEntityIsNotInitialized_WhenCallDelete_ThenShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => m_model.Delete());
        }

        [Test]
        public void GivenModelIsNew_WhenCallDelete_ThenShouldThrowInvalidOperationException()
        {
            m_model.New();

            Assert.Throws<InvalidOperationException>(() => m_model.Delete());
        }

        [Test]
        public void GivenModelEntityIsInitializedAndModelIsNotNew_WhenCallDelete_ThenShouldCallDeleteOfCommandService()
        {
            m_model.SetEntity(m_entity);

            m_model.Delete();

            m_commandServiceMock.Verify(x => x.Delete(m_entity), Times.Once);
        }

        [Test]
        public void GivenDeleteQuerySucceeds_WhenCallDelete_ThenShouldNotReInitializeModelEntity()
        {
            m_model.SetEntity(m_entity);

            m_model.Delete();

            Assert.That(m_model.IsValid, Is.False);
        }

        [Test]
        public void GivenModelIsValid_WhenCallSetReference_ThenShouldInitializeModelEntityPropertyWithValueFromSuppliedReference()
        {
            m_entity.ParentEntity = new TestEntity { Id = 12};
            m_model.SetEntity(m_entity);
            var newParent = new TestModel(m_queryServiceMock.Object);
            var newParentEntity = new TestEntity { Id = 23 };
            newParent.SetEntity(newParentEntity);
            Assert.That(m_model.Parent, Is.Not.SameAs(newParent));

            m_model.Parent = newParent;

            Assert.That(m_model.Parent, Is.SameAs(newParent));
            Assert.That(((SingleModel<TestEntity>)m_model.Parent).ToEntity(), Is.SameAs(newParentEntity));
        }

        private class TestModel2 : EditableModel<TestEntity>, ITestModel2
        {
            public TestModel2(IQueryService queryService, ICommandService commandService)
                : base(queryService, commandService)
            {
            }

            public new ICommandService CommandService
            {
                get { return base.CommandService; }
            }

            private ITestModel m_parent;
            public ITestModel Parent
            {
                get
                {
                    return m_parent ?? (m_parent = GetReference<ITestModel, TestEntity>(x => x.ParentEntity));
                }
                set
                {
                    m_parent = value;
                    SetReference(value, x => x.ParentEntity);
                }
            }
        }
    }

    public interface ITestModel2 : IEditableModel
    {
        ITestModel Parent { get; set; }
    }
}