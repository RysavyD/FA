namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HelpEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Help",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Question = c.String(nullable: false),
                        Answer = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Jsou akce zdarma?',"
                + "N'Ano i ne. U každé akce je uvedeno, jestli je akce zdarma nebo se na ní vybírá "
                + "poplatek (vstupné, doprava, ubytování, pronájem, ...).')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Mohu si uveřejnit svoji akci?',"
                + "N'Samozřejmě, každý zde může vytvořit a organizovat svoji akci a lákat na ní další uživatele.')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Mám se bát přijít na akci?',"
                + "N'Naprosto ne, akce nejsou nebezpečné, pokud to není uvedeno v popisku. Fungujeme je otevřená komunita pro všechny nové účastníky')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Co potřebuji, abych mohl navštívit akci?',"
                + "N'Stačí si jen vybrat akci, které se chci zúčastnit. Pokud nejsem zde registrovaný, tak se zaregistrovat a přihlásit se na vybranou akci.')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Co potřebuji, abych mohl navštívit předplacenou akci? Kde zjistím svůj osobní variabilní symbol?',"
                + "N'Každý uživatel dostane svůj variabilní symbol při prvním přihlášení na placenou akci. Jeho hodnotu nalezne ve svém <b><a href=\"/Profil/Detail\">profilu</a></b>')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Jak si změnit profil',"
                + "N'Na stránce profilu kliknout na tlačítko \"Editovat profil\" nebo použít tento <b><a href=\"/Profil/Nastaveni\">odkaz</a></b>, změnit vybrané položky a kliknout na tlačítko \"Uložit\"')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Jak se přihlásit na akci?',"
                + "N'Po registraci a přihlášení na stránky se objeví v detailu akce tlačítka, pomocí kterých se přihlašuje na zobrazenou akci')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Jak si změnit profilovou fotku?',"
                + "N'Na stránce profilu kliknout na tlačítko \"Nahrát profilovou fotku\" nebo použít tento <b><a href=\"/Profil/Fotka\">odkaz</a></b>."
                + "<br />Kliknout na tlačítko \"Browse\", vybrat soubor na disku a poté kliknou na tlačítko \"Vložit fotku\".')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Jak nahrát fotogalerii k akci?',"
                + "N'Přidávat fotogalerii k akci mohou jen uživatelé, kteří tuto akci navštívili.<br />"
                + "Nejprve je třeba najít akci v seznamu již <b><a href=\"/Home/Uplynule\">uskutečněných akcí</a></b>.<br />"
                + "V deatilu akce je poté tlačítko \"Přidat fotogalerii\" <br />"
                + "Do nového formuláře pak vložit odkaz na stránku z rajce.net, kde jsou nahrány vaše fotky a poté kliknout na \"Vložit album\".')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Přijde se mnou kamarád(-ka) na akci, jak ji postupovat? Jak přidat nebo odebrat externí účastníky na akci?',"
                + "N'V detailu akce je tlačítko \"Správa externistů\"<br/>"
                + "Zde je možno spravovat(přidávat či ubírat) svoje externí účastníky na dané akci <br />"
                + "Pokud se jedná o akce placenou předem, je cena za akci stržena z vašeho účtu.')");
            Sql(@"INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES ("
                + "N'Chci se ještě na něco zeptat?',"
                + "N'Kontaktujte nás přímo mailem na <b>info(zavináč)fungujemeaktivne.cz</b> nebo pište do <b><a href=\"/Diskuze/Detail/Rozvoj-webu\">diskuze o webu</a></b>')");
        }

        public override void Down()
        {
            DropTable("dbo.Help");
        }
    }
}
