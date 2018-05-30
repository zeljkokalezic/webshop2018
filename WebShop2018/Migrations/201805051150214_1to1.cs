namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1to1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Kategorijas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Naziv = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Proizvods", "Kategorija_Id", c => c.Int());
            CreateIndex("dbo.Proizvods", "Kategorija_Id");
            AddForeignKey("dbo.Proizvods", "Kategorija_Id", "dbo.Kategorijas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Proizvods", "Kategorija_Id", "dbo.Kategorijas");
            DropIndex("dbo.Proizvods", new[] { "Kategorija_Id" });
            DropColumn("dbo.Proizvods", "Kategorija_Id");
            DropTable("dbo.Kategorijas");
        }
    }
}
