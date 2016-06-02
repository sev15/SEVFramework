using System;
using LightInject;

namespace SEV.DI.LightInject
{
    internal class LightInjectContainer : ServiceContainer, IDIContainer
    {
        public LightInjectContainer() : base(new ContainerOptions { EnableVariance = false })
        {
        }

        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }

        public void DisablePropertyInjection()
        {
            PropertyDependencySelector = new NullPropertyDependencySelector();
        }
    }
}