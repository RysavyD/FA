namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextPermissions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Text", "Title", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.Text", "EditPermissions", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.Text", "ViewPermissions", c => c.String(maxLength: 150));
            AddColumn("dbo.Text", "OriginalUrl", c => c.String(maxLength: 50));
            AddColumn("dbo.Text", "Icon", c => c.String(maxLength: 50));
            Sql("Update [Text] Set Title='Badmintonové turnaje', EditPermissions='BadmintonAdmin', ViewPermissions='*', OriginalUrl='~/Badminton', Icon='icon-rocket' where [Key]='BadmintonTurnaje'");
            Sql("INSERT INTO [Text] Values('Spolecne-aktivity-zs', 'Společné aktivity z.s.', 'Společné aktivity z.s.','Council,Supervisor', Null, '~/Sdruzeni/Spolecne-aktivity-zs','icon-heart')");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Text", "Icon");
            DropColumn("dbo.Text", "OriginalUrl");
            DropColumn("dbo.Text", "ViewPermissions");
            DropColumn("dbo.Text", "EditPermissions");
            DropColumn("dbo.Text", "Title");
        }
    }
}
