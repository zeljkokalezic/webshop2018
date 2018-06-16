namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1toN : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Slikes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Naziv = c.String(),
                        Opis = c.String(),
                        Proizvod_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Proizvods", t => t.Proizvod_Id)
                .Index(t => t.Proizvod_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Slikes", "Proizvod_Id", "dbo.Proizvods");
            DropIndex("dbo.Slikes", new[] { "Proizvod_Id" });
            DropTable("dbo.Slikes");
        }
    }
}
