namespace SEV.FWK.Service.Tests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TestEntity", "CategoryId", c => c.Int());
            CreateIndex("dbo.TestEntity", "CategoryId");
            AddForeignKey("dbo.TestEntity", "CategoryId", "dbo.TestCategory", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestEntity", "CategoryId", "dbo.TestCategory");
            DropIndex("dbo.TestEntity", new[] { "CategoryId" });
            DropColumn("dbo.TestEntity", "CategoryId");
            DropTable("dbo.TestCategory");
        }
    }
}
