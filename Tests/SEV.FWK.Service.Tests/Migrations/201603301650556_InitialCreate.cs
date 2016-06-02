namespace SEV.FWK.Service.Tests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestEntity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestEntity", t => t.ParentId)
                .Index(t => t.ParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestEntity", "ParentId", "dbo.TestEntity");
            DropIndex("dbo.TestEntity", new[] { "ParentId" });
            DropTable("dbo.TestEntity");
        }
    }
}
