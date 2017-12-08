using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.ComponentModel;
using SEV.Domain.Model;

namespace SEV.UI.Model.Tests
{
    public interface ITest_Model : IEditableModel
    {
        string Value1 { get; set; }
        decimal Value2 { get; set; }
        DateTime? Value3 { get; set; }
        ITest_Model Parent { get; set; }
    }

    [TestFixture]
    public class NotifyPropertyChangedTests
    {
        private const string IdValue = "1";
        private const string Value1 = "value1";
        private const decimal Value2 = 123m;
        private static readonly DateTime Value3 = DateTime.Now;
        private static readonly Test_Entity ParentValue = new Test_Entity { Id = 2, Value1 = "value2" };

        private int m_pcCount;

        private Mock<IQueryService> m_queryServiceMock;
        private Mock<ICommandService> m_commandServiceMock;
        private Mock<IValidationService> m_validationServiceMock;
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private ITest_Model m_model;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_model = ServiceLocator.Current.GetInstance<ITest_Model>();
            m_model.Load(IdValue);
            m_model.PropertyChanged += CountPropertyChanged;
        }

        [TearDown]
        public void CleanUp()
        {
            m_model.PropertyChanged -= CountPropertyChanged;
        }

        private void InitMocks()
        {
            m_pcCount = 0;

            m_queryServiceMock = new Mock<IQueryService>();
            m_commandServiceMock = new Mock<ICommandService>();
            m_validationServiceMock = new Mock<IValidationService>();

            m_queryServiceMock.Setup(x => x.FindById<Test_Entity>(IdValue)).Returns(new Test_Entity
            {
                Id = Int32.Parse(IdValue),
                Value1 = Value1,
                Value2 = Value2,
                Value3 = Value3,
                Parent = ParentValue
            });
            m_queryServiceMock.Setup(x => x.FindById<Test_Entity>("2")).Returns(new Test_Entity { Id = 2 });

            m_serviceLocatorMock = new Mock<IServiceLocator>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<ITest_Model>()).Returns(() =>
                new Test_Model(m_queryServiceMock.Object, m_commandServiceMock.Object, m_validationServiceMock.Object));
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
        }

        private void CountPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            m_pcCount++;
        }

        #endregion

        [Test]
        public void WhenSetValue1_ThenShouldRaisePropertyChangedEvent()
        {
            m_model.Value1 = "new value";

            Assert.That(m_pcCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenSameValue1_WhenSetValue1_ThenShouldNotRaisePropertyChangedEvent()
        {
            m_model.Value1 = Value1;

            Assert.That(m_pcCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenUpdatingNullToNull_WhenSetValue1_ThenShouldNotRaisePropertyChangedEvent()
        {
            m_model.Value1 = null;
            Assert.That(m_pcCount, Is.EqualTo(1));

            m_model.Value1 = null;

            Assert.That(m_pcCount, Is.EqualTo(1));
        }

        [Test]
        public void WhenSetValue2_ThenShouldRaisePropertyChangedEvent()
        {
            m_model.Value2 = 321m;

            Assert.That(m_pcCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenSameValue2_WhenSetValue2_ThenShouldNotRaisePropertyChangedEvent()
        {
            m_model.Value2 = Value2;

            Assert.That(m_pcCount, Is.EqualTo(0));
        }

        [Test]
        public void WhenSetValue3_ThenShouldRaisePropertyChangedEvent()
        {
            m_model.Value3 = DateTime.Now;

            Assert.That(m_pcCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenSameValue3_WhenSetValue3_ThenShouldNotRaisePropertyChangedEvent()
        {
            m_model.Value3 = Value3;

            Assert.That(m_pcCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenUpdatingNullToNull_WhenSetValue3_ThenShouldNotRaisePropertyChangedEvent()
        {
            m_model.Value3 = null;
            Assert.That(m_pcCount, Is.EqualTo(1));

            m_model.Value3 = null;

            Assert.That(m_pcCount, Is.EqualTo(1));
        }

        [Test]
        public void WhenSetParentValue_ThenShouldRaisePropertyChangedEvent()
        {
            var model = ServiceLocator.Current.GetInstance<ITest_Model>();
            model.Load("2");

            m_model.Parent = model;

            Assert.That(m_pcCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenSameParentValue_WhenSetParentValue_ThenShouldNotRaisePropertyChangedEvent()
        {
            var sameParent = m_model.Parent;

            m_model.Parent = sameParent;

            Assert.That(m_pcCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenUpdatingNullToNull_WhenSetParentValue_ThenShouldNotRaisePropertyChangedEvent()
        {
            m_model.Parent = null;
            Assert.That(m_pcCount, Is.EqualTo(1));

            m_model.Parent = null;

            Assert.That(m_pcCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenAssigningInvalidModel_WhenSetParentValue_ThenShouldThrowInvalidOperationException()
        {
            var model = ServiceLocator.Current.GetInstance<ITest_Model>();

            Assert.That(() => m_model.Parent = model,
                Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(Resources.AssignInvalidModelMsg));
        }

        #region Private classes
      
        private class Test_Entity : Entity
        {
// ReSharper disable MemberHidesStaticFromOuterClass
            public string Value1 { get; set; }
            public decimal Value2 { get; set; }
            public DateTime? Value3 { get; set; }
// ReSharper disable UnusedAutoPropertyAccessor.Local
            public Test_Entity Parent { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        private class Test_Model : EditableModel<Test_Entity>, ITest_Model
        {
            public Test_Model(IQueryService queryService, ICommandService commandService, IValidationService validationService)
                : base(queryService, commandService, validationService)
            {
            }

            public string Value1
            {
                get { return GetValue(x => x.Value1); }
                set { SetValue(value); }
            }

            public decimal Value2
            {
                get { return GetValue(x => x.Value2); }
                set { SetValue(value); }
            }

            public DateTime? Value3
            {
                get { return GetValue(x => x.Value3); }
                set { SetValue(value); }
            }
// ReSharper restore MemberHidesStaticFromOuterClass

            public ITest_Model Parent
            {
                get { return GetReference<ITest_Model, Test_Entity>(); }
                set { SetReference<ITest_Model, Test_Entity>(value); }
            }
        }

        #endregion
    }
}