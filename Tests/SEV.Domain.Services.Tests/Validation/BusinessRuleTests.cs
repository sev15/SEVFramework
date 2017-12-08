using System.ComponentModel.DataAnnotations;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Services.Validation;

namespace SEV.Domain.Services.Tests.Validation
{
    [TestFixture]
    public class BusinessRuleTests
    {
        private const string TestMessage = " test error message";
        private const DomainEvent TestEvent = DomainEvent.Create;

        private Mock<BusinessRule<Entity>> m_ruleMock;

        [SetUp]
        public void Init()
        {
            m_ruleMock = new Mock<BusinessRule<Entity>>(TestMessage, TestEvent) { CallBase = true };
        }

        [Test]
        public void WhenCreateBusinessRule_ThenShouldInitializeErrorMessageProperty()
        {
            var result = m_ruleMock.Object.ErrorMessage;

            Assert.That(result, Is.EqualTo(TestMessage));
        }

        [Test]
        public void WhenCreateBusinessRule_ThenShouldInitializeRuleScopeProperty()
        {
            var result = m_ruleMock.Object.RuleScope;

            Assert.That(result, Is.EqualTo(TestEvent));
        }

        [Test]
        public void WhenCallValidate_ThenShouldCallInternalValidateEntity()
        {
            var entity = new Mock<Entity>().Object;
            m_ruleMock.Protected().Setup<bool>("ValidateEntity", entity).Returns(true);

            bool result = m_ruleMock.Object.Validate(new ValidationContext(entity));

            Assert.That(result, Is.EqualTo(true));
            m_ruleMock.Protected().Verify<bool>("ValidateEntity", Times.Once(), entity);
        }
    }
}
