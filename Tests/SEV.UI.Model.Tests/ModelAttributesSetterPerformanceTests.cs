using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace SEV.UI.Model.Tests
{
    [TestFixture]
    public class ModelAttributesSetterPerformanceTests
    {
        private const int TestCount1 = 20;
        private const int TestCount2 = 1000;

        private TestEntity testObject;
// ReSharper disable NotAccessedField.Local
        private object testResult;
// ReSharper restore NotAccessedField.Local

        #region SetUp

        [SetUp]
        public void Init()
        {
            testObject = new TestEntity
            {
                Value = "test comments",
                Parent = new TestEntity { Value = "test parent" }
            };
        }

        private TestEntity TestMethod1()
        {
            return testObject.Parent;
        }

        private TestEntity TestMethod2(string propertyName)
        {
            var propertyInfo = testObject.GetType().GetProperty(propertyName);

            return (TestEntity)propertyInfo.GetValue(testObject);
        }

        private TestEntity TestMethod3(Func<TestEntity, TestEntity> getter)
        {
            return getter(testObject);
        }

        private TestEntity TestMethod4(Expression<Func<TestEntity, TestEntity>> expr)
        {
            var entityProperty = (PropertyInfo)((MemberExpression)expr.Body).Member;

            return (TestEntity)entityProperty.GetValue(testObject);
        }

        #endregion

        [Test]
        public void Test1()
        {
            for (int count = 0; count < TestCount1; count++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < TestCount2; i++)
                {
                    testResult = TestMethod1();
                }
                stopwatch.Stop();

                Console.WriteLine(@"Elapsed time 1 = " + stopwatch.Elapsed);
            }
        }

        [Test]
        public void Test2()
        {
            for (int count = 0; count < TestCount1; count++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < TestCount2; i++)
                {
                    testResult = TestMethod2("Parent");
                }
                stopwatch.Stop();

                Console.WriteLine(@"Elapsed time 2 = " + stopwatch.Elapsed);
            }
        }

        [Test]
        public void Test3()
        {
            for (int count = 0; count < TestCount1; count++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < TestCount2; i++)
                {
                    testResult = TestMethod3(x => x.Parent);
                }
                stopwatch.Stop();

                Console.WriteLine(@"Elapsed time 3 = " + stopwatch.Elapsed);
            }
        }

        [Test]
        public void Test4()
        {
            for (int count = 0; count < TestCount1; count++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < TestCount2; i++)
                {
                    testResult = TestMethod4(x => x.Parent);
                }
                stopwatch.Stop();

                Console.WriteLine(@"Elapsed time 4 = " + stopwatch.Elapsed);
            }
        }
    }
}
