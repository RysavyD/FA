namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnifyPostsAndFileDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileUploadInfo", "Description", c => c.String(nullable: false));
            AddColumn("dbo.Post", "EditPermissions", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.Post", "ViewPermissions", c => c.String(maxLength: 150));
            AddColumn("dbo.Post", "OriginalUrl", c => c.String(maxLength: 50));
            AddColumn("dbo.Post", "Icon", c => c.String(maxLength: 50));

            Sql("INSERT INTO [Post] SELECT [Title] AS Name, [Key] AS[HtmlName], [Value] AS Content, [EditPermissions], [ViewPermissions],[OriginalUrl],[Icon] FROM [Text]");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Post", "Icon");
            DropColumn("dbo.Post", "OriginalUrl");
            DropColumn("dbo.Post", "ViewPermissions");
            DropColumn("dbo.Post", "EditPermissions");
            DropColumn("dbo.FileUploadInfo", "Description");
        }
    }
}
