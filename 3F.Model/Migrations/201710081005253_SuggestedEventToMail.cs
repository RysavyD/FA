namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuggestedEventToMail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "SendNewSuggestedEventToMail", c => c.Boolean(nullable: false));
            Sql("UPDATE [dbo].[Profiles] SET [SendNewSuggestedEventToMail] = 1 WHERE [SendNewActionToMail] = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "SendNewSuggestedEventToMail");
        }
    }
}
