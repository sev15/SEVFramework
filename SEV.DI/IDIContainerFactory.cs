using Microsoft.Practices.ServiceLocation;

namespace SEV.DI
{
    public interface IDIContainerFactory
    {
        IDIContainer CreateContainer(bool enablePropertyInjection = false);
        IServiceLocator CreateServiceLocator(IDIContainer container);
    }
}