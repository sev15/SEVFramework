using Microsoft.Practices.ServiceLocation;

namespace SEV.DI.Web
{
    public static class DIContainerExtensions
    {
        public static IDIContainerWebConfigurator GetWebConfigurator(this IDIContainer container)
        {
            var webConfigurator = ServiceLocator.Current.GetInstance<IDIContainerWebConfigurator>();
            webConfigurator.SetContainer(container);

            return webConfigurator;
        }
    }
}