namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VopVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "VopVersion", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "VopVersion");
        }
    }
}
