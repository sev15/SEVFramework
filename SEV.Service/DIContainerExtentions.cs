using SEV.DI;
using SEV.Domain.Services.Validation;
using SEV.Service.Contract;

namespace SEV.Service.DI
{
    public static class DIContainerExtentions
    {
        public static IDIContainer RegisterAplicationServices(this IDIContainer container)
        {
            container.Register<IQueryService, QueryService>();
            container.Register<ICommandService, CommandService>();
            container.Register<IValidationService, ValidationService>();
            container.Register<IBusinessRuleProvider, BusinessRuleProvider>();
            container.Register(typeof(IQuery<>), typeof(SimpleQuery<>));

            return container;
        }
    }
}