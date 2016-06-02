using LightInject;

namespace SEV.DI.LightInject
{
    internal class NullPropertyDependencySelector : IPropertyDependencySelector
    {
        public System.Collections.Generic.IEnumerable<PropertyDependency> Execute(System.Type type)
        {
            return new PropertyDependency[0];
        }
    }
}