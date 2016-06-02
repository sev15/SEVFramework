using Moq;
using NUnit.Framework;
using SEV.Domain.Model;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFRepositoryFactoryTests
    {
        [Test]
        public void WhenCallCreate_ThenShouldReturnInstanceOfEFRepository()
        {
            var context = new Mock<IDbContext>().Object;
            IRepositoryFactory factory = new EFRepositoryFactory();

            var result = factory.Create(typeof(Entity), context);

            Assert.That(result, Is.InstanceOf<EFRepository<Entity>>());
        }
    }
}