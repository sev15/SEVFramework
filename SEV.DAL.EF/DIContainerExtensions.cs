using SEV.DI;
using SEV.Domain.Services;
using SEV.Domain.Services.Validation;

namespace SEV.DAL.EF.DI
{
    public static class DIContainerExtensions
    {
        public static IDIContainer RegisterDomainServices(this IDIContainer container)
        {
            container.Register<IUnitOfWorkFactory, EFUnitOfWorkFactory>();
            container.Register<IRepositoryFactory, EFRepositoryFactory>();
            container.Register<IBusinessRuleProvider, BusinessRuleProvider>();

            return container;
        }
    }
}