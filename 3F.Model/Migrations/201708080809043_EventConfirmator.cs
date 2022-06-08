namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventConfirmator : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[AspNetRoles] VALUES('EventConfirmator')");
            Sql("CREATE NONCLUSTERED INDEX [UndecidedEventsIndex] ON[dbo].[Event]([State],[StopDateTime]) INCLUDE([Id],[EventType])");
        }

        public override void Down()
        {
            Sql("DELETE FROM [dbo].[AspNetUserRoles] WHERE [RoleId]=12");
            Sql("DELETE FROM [dbo].[AspNetRoles] WHERE [Name]='EventConfirmator'");
            Sql("DROP INDEX [UndecidedEventsIndex] ON [dbo].[Event]");
        }
    }
}
