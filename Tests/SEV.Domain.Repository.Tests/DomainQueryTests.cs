using NUnit.Framework;

namespace SEV.Domain.Repository.Tests
{
    [TestFixture]
    public class DomainQueryTests
    {
        [Test]
        public void CanStoreAndSearchValueByKey()
        {
            const string key = "TTT";
            const decimal value = 1234m;
            IDomainQuery query = new DomainQuery();

            query[key] = value;

            Assert.That(query[key], Is.EqualTo(value));
        }
    }
}