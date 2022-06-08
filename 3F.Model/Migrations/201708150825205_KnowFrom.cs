namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KnowFrom : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KnowFrom",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            Sql("INSERT INTO [KnowFrom] VALUES ('Nevím', 1)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Kamarád/kamarádka', 2)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Na Facebooku', 3)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Odkaz na webu', 4)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Z doslechu', 5)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Na akci', 6)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Inzerát', 7)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Leták / vizitka', 8)");
            Sql("INSERT INTO [KnowFrom] VALUES ('Tričko', 9)");

        }
        
        public override void Down()
        {
            DropTable("dbo.KnowFrom");
        }
    }
}
