using SEV.DAL.EF;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SEV.FWK.Service.Tests
{
    public class TestDbContext : SEVDbContext
    {
        public TestDbContext() : base("TestDatabase")
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<TestEntity>()
                        .HasOptional(s => s.Parent)
                        .WithMany(m => m.Children)
                        .Map((x) => x.MapKey("ParentId"));

            modelBuilder.Entity<TestEntity>()
                        .HasOptional(s => s.Category)
                        .WithMany(c => c.Children)
                        .Map(k => k.MapKey("CategoryId"));
        }
    }
}