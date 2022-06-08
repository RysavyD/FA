namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscussionDateTimes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Discussion", "CreateDate", c => c.DateTime());
            AddColumn("dbo.Discussion", "LastUpdateDate", c => c.DateTime());
            Sql("DELETE FROM [Discussion] WHERE IsAlone = 1 AND ((SELECT COUNT (*) FROM [DiscussionItem] WHERE Id_Discussion = [Discussion].Id) = 0)");
            Sql("UPDATE [Discussion] SET [CreateDate] = DATEADD(SECOND, -1, (SELECT TOP 1 [DateTime] FROM  [DiscussionItem] di WHERE di.Id_Discussion = [Discussion].Id ORDER BY [DateTime]))");
            Sql("UPDATE [Discussion] SET [LastUpdateDate] = (SELECT TOP 1 [DateTime] FROM  [DiscussionItem] di WHERE di.Id_Discussion = [Discussion].Id ORDER BY [DateTime] DESC)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Discussion", "LastUpdateDate");
            DropColumn("dbo.Discussion", "CreateDate");
        }
    }
}
