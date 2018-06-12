namespace WebShop2018.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dobavljac",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ime = c.String(),
                        Aktivan = c.Boolean(nullable: false),
                        Email = c.String(),
                        WebsiteURL = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Proizvod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Stanje = c.Int(nullable: false),
                        Naziv = c.String(nullable: false, maxLength: 50),
                        Cena = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SlikaZaPrikazId = c.Int(),
                        Kategorija_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kategorija", t => t.Kategorija_Id)
                .Index(t => t.Kategorija_Id);
            
            CreateTable(
                "dbo.Kategorija",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Naziv = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Slika",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Naziv = c.String(),
                        Opis = c.String(),
                        Proizvod_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Proizvod", t => t.Proizvod_Id)
                .Index(t => t.Proizvod_Id);
            
            CreateTable(
                "dbo.OrderLine",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Item_Id = c.Int(),
                        Order_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Proizvod", t => t.Item_Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .Index(t => t.Item_Id)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Comment = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        State = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Ime = c.String(),
                        Prezime = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ProizvodDobavljac",
                c => new
                    {
                        Proizvod_Id = c.Int(nullable: false),
                        Dobavljac_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Proizvod_Id, t.Dobavljac_Id })
                .ForeignKey("dbo.Proizvod", t => t.Proizvod_Id, cascadeDelete: true)
                .ForeignKey("dbo.Dobavljac", t => t.Dobavljac_Id, cascadeDelete: true)
                .Index(t => t.Proizvod_Id)
                .Index(t => t.Dobavljac_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Order", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderLine", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.OrderLine", "Item_Id", "dbo.Proizvod");
            DropForeignKey("dbo.Slika", "Proizvod_Id", "dbo.Proizvod");
            DropForeignKey("dbo.Proizvod", "Kategorija_Id", "dbo.Kategorija");
            DropForeignKey("dbo.ProizvodDobavljac", "Dobavljac_Id", "dbo.Dobavljac");
            DropForeignKey("dbo.ProizvodDobavljac", "Proizvod_Id", "dbo.Proizvod");
            DropIndex("dbo.ProizvodDobavljac", new[] { "Dobavljac_Id" });
            DropIndex("dbo.ProizvodDobavljac", new[] { "Proizvod_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Order", new[] { "User_Id" });
            DropIndex("dbo.OrderLine", new[] { "Order_Id" });
            DropIndex("dbo.OrderLine", new[] { "Item_Id" });
            DropIndex("dbo.Slika", new[] { "Proizvod_Id" });
            DropIndex("dbo.Proizvod", new[] { "Kategorija_Id" });
            DropTable("dbo.ProizvodDobavljac");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Order");
            DropTable("dbo.OrderLine");
            DropTable("dbo.Slika");
            DropTable("dbo.Kategorija");
            DropTable("dbo.Proizvod");
            DropTable("dbo.Dobavljac");
        }
    }
}
