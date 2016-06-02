using Microsoft.Practices.ServiceLocation;
using System;

namespace SEV.DI
{
    public static class ServiceLocatorExtensions
    {

        public static object Resolve(this IServiceLocator locator, Type type, string key = null)
        {
            try
            {
                return string.IsNullOrEmpty(key) ? locator.GetInstance(type) : locator.GetInstance(type, key);
            }
            catch (ActivationException)
            {
                return null;
            }
        }

        public static TService Resolve<TService>(this IServiceLocator locator, string key = null)
            where TService : class
        {
            return (TService)Resolve(locator, typeof(TService), key);
        }
    }
}
