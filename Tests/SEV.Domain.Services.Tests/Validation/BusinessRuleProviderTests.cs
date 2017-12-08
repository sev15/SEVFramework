using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Services.Validation;

namespace SEV.Domain.Services.Tests.Validation
{
    [TestFixture]
    public class BusinessRuleProviderTests
    {
        private const DomainEvent TestEvent = DomainEvent.Delete;

        private Mock<IServiceLocator> m_serviceLocatorMock;
        private IBusinessRuleProvider m_ruleProvider;

        [SetUp]
        public void Init()
        {
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);

            m_ruleProvider = new BusinessRuleProvider();
        }

        [Test]
        public void WhenCallGetBusinessRules_ThenShouldCallGetAllInstancesOfServiceLocatorForBusinessRule()
        {
            m_ruleProvider.GetBusinessRules<Entity>(TestEvent);

            m_serviceLocatorMock.Verify(x => x.GetAllInstances<BusinessRule<Entity>>(), Times.Once);
        }

        [Test]
        public void GivenNoBusinessRulesFound_WhenCallGetBusinessRules_ThenShouldReturnEmptyCollection()
        {
            IEnumerable<BusinessRule<Entity>> result = m_ruleProvider.GetBusinessRules<Entity>(TestEvent);

            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void GivenNoBusinessRulesFoundForSpecifiedDomainEvent_WhenCallGetBusinessRules_ThenShouldReturnEmptyCollection()
        {
            var rule = new Mock<BusinessRule<Entity>>("test", DomainEvent.Update).Object;
            m_serviceLocatorMock.Setup(x => x.GetAllInstances<BusinessRule<Entity>>()).Returns(new[] { rule });

            IEnumerable<BusinessRule<Entity>> result = m_ruleProvider.GetBusinessRules<Entity>(TestEvent);

            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void GivenBusinessRulesFound_WhenCallGetBusinessRules_ThenShouldReturnFoundBusinessRules()
        {
            var rule = new Mock<BusinessRule<Entity>>("test", DomainEvent.Update | TestEvent).Object;
            m_serviceLocatorMock.Setup(x => x.GetAllInstances<BusinessRule<Entity>>()).Returns(new[] { rule });

            IEnumerable<BusinessRule<Entity>> result = m_ruleProvider.GetBusinessRules<Entity>(TestEvent);

// ReSharper disable PossibleMultipleEnumeration
            Assert.That(result.Any(), Is.True);
            Assert.That(result.Single(), Is.SameAs(rule));
// ReSharper restore PossibleMultipleEnumeration
        }

        [Test]
        public void GivenActivationExceptionIsThrown_WhenCallGetBusinessRules_ThenShouldReturnEmptyCollection()
        {
            m_serviceLocatorMock.Setup(x => x.GetAllInstances<BusinessRule<Entity>>()).Throws<ActivationException>();

            IEnumerable<BusinessRule<Entity>> result = m_ruleProvider.GetBusinessRules<Entity>(TestEvent);

            Assert.That(result.Any(), Is.False);
        }
    }
}
