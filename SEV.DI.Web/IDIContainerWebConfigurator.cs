using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

namespace SEV.DI.Web
{
    public interface IDIContainerWebConfigurator
    {
        void SetContainer(IDIContainer container);
        void RegisterForWeb<TService, TImplementation>() where TImplementation : TService;
        IDependencyResolver CreateDependencyResolver();
        void EnableWeb(Assembly controllersAssembly);
        void EnableWebApi(Assembly controllersAssembly, HttpConfiguration httpConfiguration);
        void RegisterApiControllers(Assembly controllersAssembly);
    }
}