namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventCategory", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventCategory", "Description");
        }
    }
}
