namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionAtr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Images", "Description");
        }
    }
}
