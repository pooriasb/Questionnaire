namespace QuestionnaireProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SexAndNationalId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "National_Id", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Sex", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Sex");
            DropColumn("dbo.AspNetUsers", "National_Id");
        }
    }
}
