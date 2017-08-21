using System;

namespace SEV.DI
{
    public interface IDIContainer : IDependencyRegistry, IServiceProvider
    {
    }
}