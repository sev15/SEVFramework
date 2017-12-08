using System.Collections.Generic;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Validation
{
    public interface IBusinessRuleProvider
    {
        IEnumerable<BusinessRule<TEntity>> GetBusinessRules<TEntity>(DomainEvent domainEvent) where TEntity : Entity;
    }
}
