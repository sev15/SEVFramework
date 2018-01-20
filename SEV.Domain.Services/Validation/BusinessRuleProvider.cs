using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Validation
{
    public class BusinessRuleProvider : IBusinessRuleProvider
    {
        public IEnumerable<BusinessRule<TEntity>> GetBusinessRules<TEntity>(DomainEvent domainEvent)
            where TEntity : Entity
        {
            try
            {
                var rules = ServiceLocator.Current.GetAllInstances<BusinessRule<TEntity>>();
// ReSharper disable PossibleMultipleEnumeration
                if (rules == null || !rules.Any())
                {
                    return new BusinessRule<TEntity>[0];
                }
                return rules.Where(r => r.RuleScope.HasFlag(domainEvent)).ToArray();
// ReSharper restore PossibleMultipleEnumeration
            }
            catch (ActivationException)
            {
                return new BusinessRule<TEntity>[0];
            }
        }
    }
}
