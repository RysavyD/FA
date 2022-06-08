namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewAlbumsAndSummaryNotice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "SendNewAlbumsToMail", c => c.Boolean(nullable: false));
            AddColumn("dbo.Profiles", "SendNewSummaryToMail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "SendNewSummaryToMail");
            DropColumn("dbo.Profiles", "SendNewAlbumsToMail");
        }
    }
}
