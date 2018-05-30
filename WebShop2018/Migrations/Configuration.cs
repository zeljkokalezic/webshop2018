namespace WebShop2018.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Configuration;
    using WebShop2018.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebShop2018.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WebShop2018.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // Pravimo nove role:
            // - napravimo role manager
            // - proverimo da li role postoje
            // - ako ne postoji napravi novu
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            CreateRoleIfDoesNotExist(RolesConfig.ADMIN, roleManager);
            CreateRoleIfDoesNotExist(RolesConfig.USER, roleManager);


            // Pravimo Administratora:
            // - treba nam user manager
            // - vidimo da li postoji po email-u
            // - ako ne postoji napravimo ga

            var adminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            var adminPassword = WebConfigurationManager.AppSettings["AdminPassword"];

            if (! context.Users.Any(u => u.Email == adminEmail))
            {
                //admin menadzer nam sluzi za rad sa korisnicima
                var adminManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var admin = new ApplicationUser()
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    PasswordHash = adminManager.PasswordHasher.HashPassword(adminPassword),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                context.Users.Add(admin);
                context.SaveChanges();

                // dodelimo rolu
                adminManager.AddToRole(admin.Id, RolesConfig.ADMIN);
            }

            
        }

        private static void CreateRoleIfDoesNotExist(string roleName, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }
    }
}
