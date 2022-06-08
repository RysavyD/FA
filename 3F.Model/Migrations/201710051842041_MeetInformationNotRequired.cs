namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeetInformationNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Event", "MeetDateTime", c => c.DateTime());
            AlterColumn("dbo.Event", "MeetPlace", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Event", "MeetPlace", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Event", "MeetDateTime", c => c.DateTime(nullable: false));
        }
    }
}
