namespace QuestionnaireProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewMattersToDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UniqueCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "HasPaymentRequest", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "BankId", c => c.String());
            AddColumn("dbo.AspNetUsers", "MoneyBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AspNetUsers", "NationalCardImage", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "NationalCardImage");
            DropColumn("dbo.AspNetUsers", "MoneyBalance");
            DropColumn("dbo.AspNetUsers", "BankId");
            DropColumn("dbo.AspNetUsers", "HasPaymentRequest");
            DropColumn("dbo.AspNetUsers", "UniqueCode");
        }
    }
}
