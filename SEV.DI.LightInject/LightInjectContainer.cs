using System;
using LightInject;

namespace SEV.DI.LightInject
{
    internal class LightInjectContainer : ServiceContainer, IDIContainer
    {
        public LightInjectContainer() : base(new ContainerOptions { EnableVariance = false })
        {
        }

        public new void Register<TService>()
        {
            base.Register<TService>();
        }

        public new void Register<TService, TImplementation>() where TImplementation : TService
        {
            base.Register<TService, TImplementation>();
        }

        public new void Register<TService, TImplementation>(string serviceName) where TImplementation : TService
        {
            base.Register<TService, TImplementation>(serviceName);
        }

        public new void Register(Type serviceType)
        {
            base.Register(serviceType);
        }

        public new void Register(Type serviceType, Type implementingType)
        {
            base.Register(serviceType, implementingType);
        }

        public new void RegisterInstance<TService>(TService instance)
        {
            base.RegisterInstance(instance);
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