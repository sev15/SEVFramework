using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Model;
using SEV.Domain.Services.Validation;
using SEV.Service.Contract;

namespace SEV.Service
{
    internal class ValidationService : IValidationService
    {
        private readonly IBusinessRuleProvider m_brProvider;

        public ValidationService(IBusinessRuleProvider provider)
        {
            m_brProvider = provider;
        }

        public IList<ValidationResult> ValidateEntity<TEntity>(TEntity entity, DomainEvent domainEvent)
            where TEntity : Entity
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(entity, ServiceLocator.Current, null);
            Validator.TryValidateObject(entity, validationContext, validationResults, true);

            foreach (var rule in m_brProvider.GetBusinessRules<TEntity>(domainEvent))
            {
                if (!rule.Validate(validationContext))
                {
                    validationResults.Add(new ValidationResult(rule.ErrorMessage));
                }
            }

            return validationResults;
        }
    }
}
