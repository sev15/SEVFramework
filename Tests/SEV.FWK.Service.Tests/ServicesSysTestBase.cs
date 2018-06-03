using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SEV.DAL.EF;
using SEV.DAL.EF.DI;
using SEV.DI;
using SEV.DI.LightInject;
using SEV.Service.DI;
using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;

namespace SEV.FWK.Service.Tests
{
    public class ServicesSysTestBase
    {
        protected const int ChildCount = 7;
        protected const string ChildValue = "Child";

        [SetUp]
        public virtual void Init()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(@"..\.."));

            var diConfiguration = DIConfiguration.Create(new LightInjectContainerFactory());
            IDIContainer container = diConfiguration.CreateDIContainer();
            container.RegisterDomainServices();
            container.RegisterAplicationServices();
            container.Register<IDbContext, TestDbContext>();

            ServiceLocator.SetLocatorProvider(() => diConfiguration.CreateServiceLocator(container));

            InitDatabase();
        }

        private void InitDatabase()
        {
            var context = (TestDbContext)ServiceLocator.Current.GetInstance<IDbContext>();
            context.Database.ExecuteSqlCommand("DELETE FROM TestEntity;");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('TestEntity', RESEED, 0)");
            context.TestEntities.AddOrUpdate(p => p.Value, new TestEntity { Value = "Parent" });
            context.SaveChanges();
            var parentEntity = context.TestEntities.Single();
            var entities = Enumerable.Range(1, ChildCount)
                                     .Select(x => new TestEntity
                                     {
                                         Value = ChildValue + (ChildCount + 1 - x),
                                         Parent = parentEntity
                                     }).ToArray();
            context.TestEntities.AddOrUpdate(p => p.Value, entities);
            context.SaveChanges();
        }
    }
}
