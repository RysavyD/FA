namespace _3F.Model.Migrations
{
    // vygenereovano prikazem Add-Migration ItemStatus

    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemStatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TouristCardOwner", "Id_Status", "dbo.TouristStampStatus");
            DropForeignKey("dbo.TouristStampOwner", "Id_Status", "dbo.TouristStampStatus");
            DropIndex("dbo.TouristCardOwner", new[] { "Id_Status" });
            DropIndex("dbo.TouristStampOwner", new[] { "Id_Status" });
            AddColumn("dbo.TouristCardOwner", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.TouristStampOwner", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.TouristCardOwner", "Id_Status");
            DropColumn("dbo.TouristStampOwner", "Id_Status");
            DropTable("dbo.TouristStampStatus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TouristStampStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TouristStampOwner", "Id_Status", c => c.Int(nullable: false));
            AddColumn("dbo.TouristCardOwner", "Id_Status", c => c.Int(nullable: false));
            DropColumn("dbo.TouristStampOwner", "Status");
            DropColumn("dbo.TouristCardOwner", "Status");
            CreateIndex("dbo.TouristStampOwner", "Id_Status");
            CreateIndex("dbo.TouristCardOwner", "Id_Status");
            AddForeignKey("dbo.TouristStampOwner", "Id_Status", "dbo.TouristStampStatus", "Id");
            AddForeignKey("dbo.TouristCardOwner", "Id_Status", "dbo.TouristStampStatus", "Id");
        }
    }
}
