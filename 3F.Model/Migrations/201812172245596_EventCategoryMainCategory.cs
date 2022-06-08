namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class EventCategoryMainCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventCategory", "MainCategory", c => c.Int(nullable: false, defaultValue: 1));
            DropColumn("dbo.EventCategory", "Description");
            
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Pìší', N'Pesi', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Cyklo', N'Cyklo', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Hrady/zámky', N'Hrady-zamky', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Geocaching', N'Geocaching', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Cestování', N'Cestovani', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Voda', N'Voda', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Bìžky', N'Bezky', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ferraty', N'Ferraty', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Houbaøení', N'Houbareni', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Po hradech a zámcích (série)', N'Po-hradech-a-zamcich', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Po èeských kopcích (série)', N'Po-ceskych-kopcich', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ostatní', N'Ostatni', 2)");

            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Badminton', N'Badminton', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Volejbal', N'Volejbal', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Bìh', N'Beh', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Lyže (sjezd)', N'Lyze-sjezd', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Spinning', N'Spinning', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Tenis', N'Tenis', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Fitness', N'Fitness', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Plavání', N'Plavani', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ping-pong', N'Ping-pong', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Squash', N'Squash', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Lezení', N'Lezeni', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Bruslení', N'Brusleni', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ostatní', N'Ostatni', 1)");

            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Stolní hry', N'Stolni-hry', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Posezení', N'Posezeni', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Tanec', N'Tanec', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Hudba', N'Hudba', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Muzeum', N'Muzeum', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Divadlo', N'Divadlo', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Kino', N'Kino', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Vzdìlávání', N'Vzdelavani', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ping-pong', N'Ping-pong', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Rukodìlné', N'Rukodelne', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Gastro', N'Gastro', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Wellness', N'Wellness', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Mìsto', N'Mesto', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Bowling', N'Bowling', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Šipky', N'Sipky', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Kuleèník', N'Kulecnik', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Dìti', N'Deti', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Tour po pivovarech (série)', N'Tour-po-pivovarech', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Práce', N'Prace', 3)");

            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 31 WHERE [Id_EventCategory] = 1");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 34 WHERE [Id_EventCategory] = 2");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 27 WHERE [Id_EventCategory] = 4");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 47 WHERE [Id_EventCategory] = 7");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 38 WHERE [Id_EventCategory] = 8");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 52 WHERE [Id_EventCategory] = 13");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 64 WHERE [Id_EventCategory] = 14");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 42 WHERE [Id_EventCategory] = 15");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 23 WHERE [Id_EventCategory] = 16");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 32 WHERE [Id_EventCategory] = 18");
            Sql("UPDATE [dbo].[EventCategories] SET [Id_EventCategory] = 48 WHERE [Id_EventCategory] = 19");

            Sql("DELETE FROM [dbo].[EventCategories] WHERE [Id_EventCategory] < 22");
            Sql("DELETE FROM [dbo].[EventCategory] WHERE [Id] < 22");
        }

        public override void Down()
        {
            AddColumn("dbo.EventCategory", "Description", c => c.String(nullable: false));
            DropColumn("dbo.EventCategory", "MainCategory");
        }
    }
}
