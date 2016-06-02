﻿using Moq;
using NUnit.Framework;
using SEV.DI;
using SEV.Service.Contract;

namespace SEV.Service.DI.Tests
{
    [TestFixture]
    public class DIContainerExtentionsTests
    {
        private Mock<IDIContainer> m_containerMock;
        private IDIContainer m_container;

        #region SetUp

        [SetUp]
        public void Init()
        {
            m_container = CreateDIContainer();
        }

        private IDIContainer CreateDIContainer()
        {
            var containerFactoryMock = new Mock<IDIContainerFactory>();
            m_containerMock = new Mock<IDIContainer>();
            containerFactoryMock.Setup(x => x.CreateContainer()).Returns(m_containerMock.Object);
            var configuration = DIConfiguration.Create(containerFactoryMock.Object);

            return configuration.CreateDIContainer();
        }

        #endregion

        [Test]
        public void WhenCallRegisterAplicationServices_ThenShouldCallRegisterOfDIContainerForIQueryService()
        {
            m_container.RegisterAplicationServices();

            m_containerMock.Verify(x => x.Register<IQueryService, QueryService>(), Times.Once);
        }

        [Test]
        public void WhenCallRegisterAplicationServices_ThenShouldCallRegisterOfDIContainerForICommandService()
        {
            m_container.RegisterAplicationServices();

            m_containerMock.Verify(x => x.Register<ICommandService, CommandService>(), Times.Once);
        }

        [Test]
        public void WhenCallRegisterAplicationServices_ThenShouldCallRegisterOfDIContainerForIQuery()
        {
            m_container.RegisterAplicationServices();

            m_containerMock.Verify(x => x.Register(typeof(IQuery<>), typeof(SimpleQuery<>)), Times.Once);
        }
    }
}