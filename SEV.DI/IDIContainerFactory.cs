using Microsoft.Practices.ServiceLocation;

namespace SEV.DI
{
    public interface IDIContainerFactory
    {
        IDIContainer CreateContainer();
        IServiceLocator CreateServiceLocator(IDIContainer container);
    }
}