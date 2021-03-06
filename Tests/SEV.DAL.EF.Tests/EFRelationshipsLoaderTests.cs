﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SEV.Common;
using SEV.Domain.Model;
using SEV.Domain.Services.Data;

namespace SEV.DAL.EF.Tests
{
    [TestFixture]
    public class EFRelationshipsLoaderTests
    {
        private const int TestId = 10;

        private Mock<IDbContext> m_dbContextMock;
        private Mock<IRepositoryFactory> m_repositoryFactoryMock;
        private Mock<IRepository<TestEntity>> m_repositoryMock;
        private Mock<IServiceLocator> m_serviceLocatorMock;
        private IRelationshipsLoader<TestEntity> m_relationshipManager;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_relationshipManager = new EFRelationshipsLoader<TestEntity>(m_dbContextMock.Object);
        }

        private void InitMocks()
        {
            m_dbContextMock = new Mock<IDbContext>();
            m_dbContextMock.Setup(x => x.GetEntityReferenceId(It.IsAny<TestEntity>(), "Parent")).Returns(TestId);
            m_serviceLocatorMock = new Mock<IServiceLocator>();
            m_repositoryFactoryMock = new Mock<IRepositoryFactory>();
            m_serviceLocatorMock.Setup(x => x.GetInstance<IRepositoryFactory>())
                                .Returns(m_repositoryFactoryMock.Object);
            m_repositoryMock = new Mock<IRepository<TestEntity>>();
            m_repositoryFactoryMock.Setup(x => x.Create(typeof (TestEntity), m_dbContextMock.Object))
                                   .Returns(m_repositoryMock.Object);
            m_repositoryMock.Setup(x => x.GetByIdList(It.IsAny<IEnumerable<object>>()))
                            .Returns(new List<TestEntity> { new TestEntity { Id = TestId } });
            ServiceLocator.SetLocatorProvider(() => m_serviceLocatorMock.Object);
        }

        #endregion

        [Test]
        public void GivenNavigationPropertyForSingleReference_WhenCallLoad_ForSingleEntity_ThenShouldCallLoadEntityReferenceOfDbContext()
        {
            var entity = new TestEntity();
            Expression<Func<TestEntity, object>> refExpression = x => x.Parent;

            m_relationshipManager.Load(entity, new[] { refExpression });

            m_dbContextMock.Verify(x => x.LoadEntityReference(entity, refExpression), Times.Once);
        }

        [Test]
        public void GivenNavigationPropertyForCollectionReference_WhenCallLoad_ForSingleEntity_ThenShouldCallLoadEntityCollectionOfDbContext()
        {
            var entity = new TestEntity();
            Expression<Func<TestEntity, object>> refExpression = x => x.Children;
            string propName = LambdaExpressionHelper.GetPropertyName(refExpression);

            m_relationshipManager.Load(entity, new[] { refExpression });

            m_dbContextMock.Verify(x => x.LoadEntityCollection(entity, propName), Times.Once);
        }

        [Test]
        public void GivenNavigationPropertyForCollectionReference_WhenCallLoad_ForEntityCollection_ThenShouldThrowArgumentException()
        {
            var entity = new TestEntity();
            Expression<Func<TestEntity, object>> refExpression = x => x.Children;

            Assert.Throws<ArgumentException>(() =>
                                m_relationshipManager.Load(new List<TestEntity> { entity }, new[] { refExpression }));
        }

        [Test]
        public void WhenCallLoad_ForEntityCollection_ThenShouldCallGetEntityReferenceIdOfDbContext()
        {
            const int count = 3;
            var entities = Enumerable.Range(1, count).Select(x => new TestEntity { Id = x }).ToList();
            Expression<Func<TestEntity, object>> refExpression = x => x.Parent;
            string propName = LambdaExpressionHelper.GetPropertyName(refExpression);

            m_relationshipManager.Load(entities, new[] { refExpression });

            foreach (var entity in entities)
            {
                m_dbContextMock.Verify(x => x.GetEntityReferenceId(entity, propName), Times.Once);
            }
        }

        [Test]
        public void WhenCallLoad_ForEntityCollection_ThenShouldCallGetInstanceOfServiceLocatorForRepositoryFactory()
        {
            const int count = 3;
            var entities = Enumerable.Range(1, count).Select(x => new TestEntity { Id = x }).ToList();
            Expression<Func<TestEntity, object>> refExpression = x => x.Parent;

            m_relationshipManager.Load(entities, new[] { refExpression });

            m_serviceLocatorMock.Verify(x => x.GetInstance<IRepositoryFactory>(), Times.Once);
        }

        [Test]
        public void WhenCallLoad_ForEntityCollection_ThenShouldCallCreateOfRepositoryFactory()
        {
            const int count = 3;
            var entities = Enumerable.Range(1, count).Select(x => new TestEntity { Id = x }).ToList();
            Expression<Func<TestEntity, object>> refExpression = x => x.Parent;

            m_relationshipManager.Load(entities, new[] { refExpression });
            
            m_repositoryFactoryMock.Verify(x => x.Create(typeof(TestEntity), m_dbContextMock.Object), Times.Once);
        }

        [Test]
        public void WhenCallLoad_ForEntityCollection_ThenShouldCallGetByIdListOfEntityRepository()
        {
            const int count = 3;
            var entities = Enumerable.Range(1, count).Select(x => new TestEntity { Id = x }).ToList();
            Expression<Func<TestEntity, object>> refExpression = x => x.Parent;

            m_relationshipManager.Load(entities, new[] { refExpression });

            m_repositoryMock.Verify(x => x.GetByIdList(It.Is<IEnumerable<object>>(y => (int)y.Single() == TestId)),
                                        Times.Once);
        }

        [Test]
        public void WhenCallLoad_ForEntityCollection_ThenShouldSetParentReferenceForEachEntityFromSuppliedCollection()
        {
            const int count = 3;
            var entities = Enumerable.Range(1, count).Select(x => new TestEntity { Id = x }).ToList();
            Expression<Func<TestEntity, object>> refExpression = x => x.Parent;

            m_relationshipManager.Load(entities, new[] { refExpression });

            foreach (var entity in entities)
            {
                Assert.That(entity.Parent, Is.Not.Null);
                Assert.That(entity.Parent.Id, Is.EqualTo(TestId));
            }
        }
    }

    public class TestEntity : Entity, IAggregateRoot
    {
        [Parent]
        public TestEntity Parent { get; set; }
        public ICollection<TestEntity> Children { get; set; }
    }
}