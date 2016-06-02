using LightInject;
using NUnit.Framework;
using System.Linq;

namespace SEV.DI.LightInject.Tests
{
    [TestFixture]
    public class NullPropertyDependencySelectorTests
    {
        [Test]
        public void WhenCallExecute_ThenShouldReturnEmptyCollectionOfPropertyDependency()
        {
            IPropertyDependencySelector propertyDependencySelector = new NullPropertyDependencySelector();

            var result = propertyDependencySelector.Execute(typeof(int));

            Assert.That(result.Any(), Is.False);
        }
    }
}
