namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nasledjivanje : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dobavljacs", "Email", c => c.String());
            AddColumn("dbo.Dobavljacs", "WebsiteURL", c => c.String());
            AddColumn("dbo.Dobavljacs", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dobavljacs", "Discriminator");
            DropColumn("dbo.Dobavljacs", "WebsiteURL");
            DropColumn("dbo.Dobavljacs", "Email");
        }
    }
}
