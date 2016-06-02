using System;

namespace SEV.DI
{
    // TODO: Add "Per Scope Request".
    public interface IDependencyRegistry
    {
        void Register<TService>();
        void Register<TService, TImplementation>() where TImplementation : TService;
        void Register<TService, TImplementation>(string serviceName) where TImplementation : TService;
        void Register(Type serviceType);
        void Register(Type serviceType, Type implementingType);
        //void Register(Type serviceType, Type implementingType, string serviceName);
        void RegisterInstance<TService>(TService instance);
    }
}