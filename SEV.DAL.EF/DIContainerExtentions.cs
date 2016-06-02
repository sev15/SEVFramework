using SEV.DI;
using SEV.Domain.Repository;

namespace SEV.DAL.EF.DI
{
    public static class DIContainerExtentions
    {
        public static IDIContainer RegisterDomainServices(this IDIContainer container)
        {
            container.Register<IUnitOfWorkFactory, EFUnitOfWorkFactory>();
            container.Register<IRepositoryFactory, EFRepositoryFactory>();
            container.Register<IRelatedEntitiesStateAdjuster, EFRelatedEntitiesStateAdjuster>();

            return container;
        }
    }
}