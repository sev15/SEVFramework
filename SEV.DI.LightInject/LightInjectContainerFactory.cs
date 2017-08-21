using LightInject;
using LightInject.ServiceLocation;
using Microsoft.Practices.ServiceLocation;

namespace SEV.DI.LightInject
{
    public class LightInjectContainerFactory : IDIContainerFactory
    {
        public IDIContainer CreateContainer(bool enablePropertyInjection = false)
        {
            return new LightInjectContainer(enablePropertyInjection);
        }

        public IServiceLocator CreateServiceLocator(IDIContainer container)
        {
            var serviceContainer = (IServiceContainer)container;

            return new LightInjectServiceLocator(serviceContainer);
        }
    }
}
