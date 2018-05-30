namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Validacije : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Proizvods", "Naziv", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Proizvods", "Naziv", c => c.String());
        }
    }
}
