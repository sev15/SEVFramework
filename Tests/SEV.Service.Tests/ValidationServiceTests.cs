using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Services.Validation;
using SEV.Service.Contract;

namespace SEV.Service.Tests
{
    [TestFixture]
    public class ValidationServiceTests
    {
        private const DomainEvent TestEvent = DomainEvent.Create;
        private const string TestMessage = "error message";

        private TestEntity m_entity;
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private Mock<IBusinessRuleProvider> m_providerMock;
        private IValidationService m_service;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_service = new ValidationService(m_providerMock.Object);
        }

        private void InitMocks()
        {
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
            m_entity = new TestEntity { Value = "test value" };
            m_providerMock = new Mock<IBusinessRuleProvider>();
        }

        #endregion

        [Test]
        public void WhenCallValidateEntity_ThenShouldValitateValidationAttributes()
        {
            m_entity.Value = null;

            var results = m_service.ValidateEntity(m_entity, TestEvent);

            Assert.That(results.Any(), Is.True);
            Assert.That(results.Single().ErrorMessage, Is.EqualTo(TestMessage));
            Assert.That(results.Single().MemberNames.Single(), Is.EqualTo(nameof(TestEntity.Value)));
        }

        [Test]
        public void WhenCallValidateEntity_ThenShouldCallGetBusinessRulesOfBusinessRuleProvider()
        {
            m_service.ValidateEntity(m_entity, TestEvent);

            m_providerMock.Verify(x => x.GetBusinessRules<TestEntity>(TestEvent), Times.Once);
        }

        [Test]
        public void GivenSomeBusinessRuleFails_WhenCallValidateEntity_ThenShouldReturnValidationResults()
        {
            var ruleMock = new Mock<BusinessRule<TestEntity>>(TestMessage, TestEvent) { CallBase = true };
            m_providerMock.Setup(x => x.GetBusinessRules<TestEntity>(TestEvent)).Returns(new[] { ruleMock.Object });

            var results = m_service.ValidateEntity(m_entity, TestEvent);

            Assert.That(results.Any(), Is.True);
            Assert.That(results.Single().ErrorMessage, Is.EqualTo(TestMessage));
            Assert.That(results.Single().MemberNames.Any(), Is.False);
        }

        [Test]
        public void GivenAllBusinessRulesSucceed_WhenCallValidateEntity_ThenShouldNotReturnValidationResults()
        {
            var ruleMock = new Mock<BusinessRule<TestEntity>>(TestMessage, TestEvent) { CallBase = true };
            ruleMock.Protected().Setup<bool>("ValidateEntity", m_entity).Returns(true);
            m_providerMock.Setup(x => x.GetBusinessRules<TestEntity>(TestEvent)).Returns(new[] { ruleMock.Object });

            var results = m_service.ValidateEntity(m_entity, TestEvent);

            Assert.That(results.Any(), Is.False);
        }


        public class TestEntity : Entity
        {
            [Required(ErrorMessage = TestMessage)]
// ReSharper disable UnusedAutoPropertyAccessor.Global
            public string Value { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
        }
    }
}
