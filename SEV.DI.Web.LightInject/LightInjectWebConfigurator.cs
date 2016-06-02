using System.Web;
using LightInject;
using LightInject.Mvc;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using LightInject.Web;

namespace SEV.DI.Web.LightInject
{
    public class LightInjectWebConfigurator : IDIContainerWebConfigurator
    {
        private IDIContainer m_diContainer;

        public void SetContainer(IDIContainer container)
        {
            m_diContainer = container;
        }

        public void RegisterForWeb<TService, TImplementation>() where TImplementation : TService
        {
            var container = (IServiceContainer)m_diContainer;
            container.Register<TService, TImplementation>(new PerScopeLifetime());
        }

        public IDependencyResolver CreateDependencyResolver()
        {
            return new LightInjectMvcDependencyResolver((IServiceContainer)m_diContainer);
        }

        public void EnableWeb(Assembly controllersAssembly)
        {
            var container = (IServiceContainer)m_diContainer;
            container.RegisterControllers(controllersAssembly);
            container.EnableMvc();
            container.Register<IHttpModule, LightInjectHttpModule>();
        }

        public void EnableWebApi(Assembly controllersAssembly, HttpConfiguration httpConfiguration)
        {
            var container = (IServiceContainer)m_diContainer;

            container.RegisterApiControllers(controllersAssembly);
            (m_diContainer as ServiceContainer).EnablePerWebRequestScope();
            container.EnableWebApi(httpConfiguration);
        }

        public void RegisterApiControllers(Assembly controllersAssembly)
        {
            var container = (IServiceContainer)m_diContainer;
            container.RegisterApiControllers(controllersAssembly);
        }
    }
}