using System.Reflection;
using System.Web.Http;

namespace SEV.DI.Web
{
    public interface IDIContainerWebConfigurator
    {
        void SetContainer(IDIContainer container);
        void EnableWeb();
        void RegisterForWeb<TService, TImplementation>() where TImplementation : TService;
        void EnableMvc(params Assembly[] assemblies);
        void EnableWebApi(HttpConfiguration httpConfiguration, params Assembly[] assemblies);
    }
}