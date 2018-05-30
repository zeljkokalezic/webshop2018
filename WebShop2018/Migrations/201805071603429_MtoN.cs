namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MtoN : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dobavljacs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ime = c.String(),
                        Aktivan = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DobavljacProizvods",
                c => new
                    {
                        Dobavljac_Id = c.Int(nullable: false),
                        Proizvod_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Dobavljac_Id, t.Proizvod_Id })
                .ForeignKey("dbo.Dobavljacs", t => t.Dobavljac_Id, cascadeDelete: true)
                .ForeignKey("dbo.Proizvods", t => t.Proizvod_Id, cascadeDelete: true)
                .Index(t => t.Dobavljac_Id)
                .Index(t => t.Proizvod_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DobavljacProizvods", "Proizvod_Id", "dbo.Proizvods");
            DropForeignKey("dbo.DobavljacProizvods", "Dobavljac_Id", "dbo.Dobavljacs");
            DropIndex("dbo.DobavljacProizvods", new[] { "Proizvod_Id" });
            DropIndex("dbo.DobavljacProizvods", new[] { "Dobavljac_Id" });
            DropTable("dbo.DobavljacProizvods");
            DropTable("dbo.Dobavljacs");
        }
    }
}
