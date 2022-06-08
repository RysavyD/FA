namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        HtmlName = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventCategories",
                c => new
                    {
                        Id_Event = c.Int(nullable: false),
                        Id_EventCategory = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id_Event, t.Id_EventCategory })
                .ForeignKey("dbo.Event", t => t.Id_Event, cascadeDelete: true)
                .ForeignKey("dbo.EventCategory", t => t.Id_EventCategory, cascadeDelete: true)
                .Index(t => t.Id_Event)
                .Index(t => t.Id_EventCategory);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventCategories", "Id_EventCategory", "dbo.EventCategory");
            DropForeignKey("dbo.EventCategories", "Id_Event", "dbo.Event");
            DropIndex("dbo.EventCategories", new[] { "Id_EventCategory" });
            DropIndex("dbo.EventCategories", new[] { "Id_Event" });
            DropTable("dbo.EventCategories");
            DropTable("dbo.EventCategory");
        }
    }
}
