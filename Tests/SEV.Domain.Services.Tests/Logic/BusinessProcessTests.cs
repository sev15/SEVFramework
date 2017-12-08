using System;
using Moq;
using NUnit.Framework;
using SEV.Domain.Model;
using SEV.Domain.Services.Logic;

namespace SEV.Domain.Services.Tests.Logic
{
    [TestFixture]
    public class BusinessProcessTests
    {
        private const int TestOrder = 111;

        private Entity m_entity;
        private IUnitOfWork m_unitOfWork;
        private TestBusinessProcess m_businessProcess;

        [SetUp]
        public void Init()
        {
            m_entity = new Mock<Entity>().Object;
            m_unitOfWork = new Mock<IUnitOfWork>().Object;

            m_businessProcess = new TestBusinessProcess(TestOrder, m_entity, m_unitOfWork);
        }

        [Test]
        public void WhenCreateBusinessProcess_ThenShouldInitializeExecutionOrderProperty()
        {
            Assert.That(m_businessProcess.ExecutionOrder, Is.EqualTo(TestOrder));
        }

        [Test]
        public void WhenCreateBusinessProcess_ThenShouldInitializeEntityProperty()
        {
            Assert.That(m_businessProcess.Entity, Is.SameAs(m_entity));
        }

        [Test]
        public void WhenCreateBusinessProcess_ThenShouldInitializeUnitOfWorkProperty()
        {
            Assert.That(m_businessProcess.UnitOfWork, Is.SameAs(m_unitOfWork));
        }

        private class TestBusinessProcess : BusinessProcess<Entity>
        {
            public TestBusinessProcess(int order, Entity entity, IUnitOfWork unitOfWork) :
                base(order, entity, unitOfWork)
            {
            }

            public new Entity Entity => base.Entity;
            public new IUnitOfWork UnitOfWork => base.UnitOfWork;

            public override void Execute()
            {
                throw new NotImplementedException();
            }
        }
    }
}
