namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileUploadInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileUploadInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Path = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileUploadInfo");
        }
    }
}
