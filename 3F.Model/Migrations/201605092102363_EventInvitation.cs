namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventInvitation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventInvitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_Event = c.Int(nullable: false),
                        Id_User = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.Id_Event)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User)
                .Index(t => t.Id_Event)
                .Index(t => t.Id_User);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventInvitations", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventInvitations", "Id_Event", "dbo.Event");
            DropIndex("dbo.EventInvitations", new[] { "Id_User" });
            DropIndex("dbo.EventInvitations", new[] { "Id_Event" });
            DropTable("dbo.EventInvitations");
        }
    }
}
