using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SEV.Service.Contract;

namespace SEV.FWK.Service.Tests
{
    [TestFixture]
    public class SyncAsyncServicesPerformanceTests : ServicesSysTestBase
    {
        private const int TestCount1 = 12;

        [Test]
        public void SyncServicesTest()
        {
            for (int count = 0; count < TestCount1; count++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var entities = new TestEntity[ChildCount];
                var service = ServiceLocator.Current.GetInstance<IQueryService>();
                for (int i = 0; i < ChildCount; i++)
                {
                    string id = (i + 1).ToString();
                    entities[i] = service.FindById<TestEntity>(id);
                }
                Assert.That(entities.Length, Is.EqualTo(ChildCount));
                stopwatch.Stop();

                Console.WriteLine(@"Elapsed time 1 = " + stopwatch.Elapsed);
            }
        }

        [Test]
        public void AsyncServicesTest()
        {
            for (int count = 0; count < TestCount1; count++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var entities = new TestEntity[ChildCount];
                var tasks = new Task<TestEntity>[ChildCount];
                for (int i = 0; i < ChildCount; i++)
                {
                    string id = (i + 1).ToString();
                    tasks[i] = MakeAsyncCall(id);
                }
                Task.WaitAll(tasks);
                for (int i = 0; i < ChildCount; i++)
                {
                    entities[i] = tasks[i].Result;
                }
                Assert.That(entities.Length, Is.EqualTo(ChildCount));
                stopwatch.Stop();

                Console.WriteLine(@"Elapsed time 2 = " + stopwatch.Elapsed);
            }
        }

        private async Task<TestEntity> MakeAsyncCall(string id)
        {
            var service = ServiceLocator.Current.GetInstance<IQueryService>();

            return await service.FindByIdAsync<TestEntity>(id);
        }

        [Test]
        public async Task SyncAsyncServicesTest()
        {
            for (int count = 0; count < TestCount1; count++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var entities = new TestEntity[ChildCount];
                var service = ServiceLocator.Current.GetInstance<IQueryService>();
                for (int i = 0; i < ChildCount; i++)
                {
                    string id = (i + 1).ToString();
                    entities[i] = await service.FindByIdAsync<TestEntity>(id);
                }
                Assert.That(entities.Length, Is.EqualTo(ChildCount));
                stopwatch.Stop();

                Console.WriteLine(@"Elapsed time 3 = " + stopwatch.Elapsed);
            }
        }
    }
}
