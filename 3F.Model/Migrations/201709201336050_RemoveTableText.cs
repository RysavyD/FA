namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTableText : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Text");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Text",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                        Title = c.String(nullable: false, maxLength: 150),
                        EditPermissions = c.String(nullable: false, maxLength: 150),
                        ViewPermissions = c.String(maxLength: 150),
                        OriginalUrl = c.String(maxLength: 50),
                        Icon = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
