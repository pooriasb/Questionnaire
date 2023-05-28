using QuestionnaireProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace QuestionnaireProject.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {


            //var RoleManager = new RoleManager<IdentityRole>(
            //    new RoleStore<IdentityRole>(new QuestionnaireEntities()));
            //if (!RoleManager.RoleExists("Admin"))
            //{
            //    RoleManager.Create(new IdentityRole("Admin"));
            //}


            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
