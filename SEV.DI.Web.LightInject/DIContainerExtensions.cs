namespace SEV.DI.Web.LightInject
{
    public static class DIContainerExtensions
    {
        public static IDIContainer RegisterWebConfigurator(this IDIContainer container)
        {
            container.Register<IDIContainerWebConfigurator, LightInjectWebConfigurator>();

            return container;
        }
    }
}