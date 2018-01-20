using System.ComponentModel.DataAnnotations;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Validation
{
    public abstract class BusinessRule<TEntity> where TEntity : Entity
    {
        protected BusinessRule(string message, DomainEvent scope)
        {
            ErrorMessage = message;
            RuleScope = scope;
        }

        public string ErrorMessage { get; }
        public DomainEvent RuleScope { get; }

        public bool Validate(ValidationContext context)
        {
            //var serviceProvider = (IServiceProvider)context.GetService(typeof(IServiceProvider));

            return ValidateEntity((TEntity)context.ObjectInstance);
        }

        protected abstract bool ValidateEntity(TEntity obj);
    }
}
