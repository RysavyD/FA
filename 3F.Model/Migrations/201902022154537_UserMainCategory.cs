namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMainCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUsersMainCategories",
                c => new
                    {
                        Id_User = c.Int(nullable: false),
                        MainCategory = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id_User, t.MainCategory })
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User)
                .Index(t => t.Id_User);
            
            CreateTable(
                "dbo.EventCategoryAspNetUsers",
                c => new
                    {
                        EventCategory_Id = c.Int(nullable: false),
                        AspNetUsers_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventCategory_Id, t.AspNetUsers_Id })
                .ForeignKey("dbo.EventCategory", t => t.EventCategory_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.AspNetUsers_Id, cascadeDelete: true)
                .Index(t => t.EventCategory_Id)
                .Index(t => t.AspNetUsers_Id);
            
            AddColumn("dbo.Profiles", "SendEventIsStayMail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsersMainCategories", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventCategoryAspNetUsers", "AspNetUsers_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventCategoryAspNetUsers", "EventCategory_Id", "dbo.EventCategory");
            DropIndex("dbo.EventCategoryAspNetUsers", new[] { "AspNetUsers_Id" });
            DropIndex("dbo.EventCategoryAspNetUsers", new[] { "EventCategory_Id" });
            DropIndex("dbo.AspNetUsersMainCategories", new[] { "Id_User" });
            DropColumn("dbo.Profiles", "SendEventIsStayMail");
            DropTable("dbo.EventCategoryAspNetUsers");
            DropTable("dbo.AspNetUsersMainCategories");
        }
    }
}
