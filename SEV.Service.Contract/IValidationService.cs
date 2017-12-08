using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SEV.Domain.Model;

namespace SEV.Service.Contract
{
    public interface IValidationService
    {
        IList<ValidationResult> ValidateEntity<TEntity>(TEntity entity, DomainEvent domainEvent)
            where TEntity : Entity;
    }
}
