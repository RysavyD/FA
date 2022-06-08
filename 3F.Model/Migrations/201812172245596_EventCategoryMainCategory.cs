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
            
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'P��', N'Pesi', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Cyklo', N'Cyklo', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Hrady/z�mky', N'Hrady-zamky', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Geocaching', N'Geocaching', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Cestov�n�', N'Cestovani', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Voda', N'Voda', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'B�ky', N'Bezky', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ferraty', N'Ferraty', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Houba�en�', N'Houbareni', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Po hradech a z�mc�ch (s�rie)', N'Po-hradech-a-zamcich', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Po �esk�ch kopc�ch (s�rie)', N'Po-ceskych-kopcich', 2)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ostatn�', N'Ostatni', 2)");

            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Badminton', N'Badminton', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Volejbal', N'Volejbal', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'B�h', N'Beh', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ly�e (sjezd)', N'Lyze-sjezd', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Spinning', N'Spinning', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Tenis', N'Tenis', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Fitness', N'Fitness', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Plav�n�', N'Plavani', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ping-pong', N'Ping-pong', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Squash', N'Squash', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Lezen�', N'Lezeni', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Bruslen�', N'Brusleni', 1)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ostatn�', N'Ostatni', 1)");

            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Stoln� hry', N'Stolni-hry', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Posezen�', N'Posezeni', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Tanec', N'Tanec', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Hudba', N'Hudba', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Muzeum', N'Muzeum', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Divadlo', N'Divadlo', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Kino', N'Kino', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Vzd�l�v�n�', N'Vzdelavani', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Ping-pong', N'Ping-pong', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Rukod�ln�', N'Rukodelne', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Gastro', N'Gastro', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Wellness', N'Wellness', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'M�sto', N'Mesto', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Bowling', N'Bowling', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'�ipky', N'Sipky', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Kule�n�k', N'Kulecnik', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'D�ti', N'Deti', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Tour po pivovarech (s�rie)', N'Tour-po-pivovarech', 3)");
            Sql("INSERT INTO [dbo].[EventCategory] VALUES (N'Pr�ce', N'Prace', 3)");

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
