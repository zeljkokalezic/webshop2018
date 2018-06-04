namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageName = c.String(),
                        Proizvod_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Proizvods", t => t.Proizvod_Id)
                .Index(t => t.Proizvod_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Images", "Proizvod_Id", "dbo.Proizvods");
            DropIndex("dbo.Images", new[] { "Proizvod_Id" });
            DropTable("dbo.Images");
        }
    }
}
