namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HtmlNameIndex : DbMigration
    {
        public override void Up()
        {
            Sql("CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170803-114530] ON [dbo].[Event]"
                + "([HtmlName] ASC"
                + ")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");
        }
        
        public override void Down()
        {
            Sql("DROP INDEX [NonClusteredIndex-20170803-114530] ON [dbo].[Event]");
        }
    }
}
