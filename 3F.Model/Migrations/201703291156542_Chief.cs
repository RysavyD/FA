namespace _3F.Model.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Chief : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[AspNetRoles] VALUES('Chief')");
            Sql("INSERT INTO [dbo].[AspNetUserRoles] VALUES (11, 297)");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM [dbo].[AspNetUserRoles] WHERE [RoleId]=11");
            Sql("DELETE FROM [dbo].[AspNetRoles] WHERE [Name]='Chief'");
        }
    }
}
