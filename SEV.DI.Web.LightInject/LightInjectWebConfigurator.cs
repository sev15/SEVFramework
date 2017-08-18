using LightInject;
using System.Reflection;
using System.Web.Http;

namespace SEV.DI.Web.LightInject
{
    public class LightInjectWebConfigurator : IDIContainerWebConfigurator
    {
        private IServiceContainer m_diContainer;

        public void SetContainer(IDIContainer container)
        {
            m_diContainer = (IServiceContainer)container;
        }

        public void EnableWeb()
        {
            (m_diContainer as ServiceContainer).EnablePerWebRequestScope();
        }

        public void RegisterForWeb<TService, TImplementation>() where TImplementation : TService
        {
            m_diContainer.Register<TService, TImplementation>(new PerScopeLifetime());
        }

        public void EnableMvc(params Assembly[] assemblies)
        {
            m_diContainer.RegisterControllers(assemblies);
            m_diContainer.EnableMvc();
        }

        public void EnableWebApi(HttpConfiguration httpConfiguration, params Assembly[] assemblies)
        {
            m_diContainer.RegisterApiControllers(assemblies);
            (m_diContainer as ServiceContainer).EnablePerWebRequestScope();
            m_diContainer.EnableWebApi(httpConfiguration);
        }
    }
}