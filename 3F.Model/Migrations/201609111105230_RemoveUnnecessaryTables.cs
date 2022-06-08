namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUnnecessaryTables : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RobotTask");
            DropTable("dbo.VersionDB");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.VersionDB",
                c => new
                    {
                        Number = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.Number, t.Date });
            
            CreateTable(
                "dbo.RobotTask",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskType = c.String(nullable: false, maxLength: 2),
                        ID_TaskType = c.String(nullable: false, maxLength: 40),
                        Finished = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
