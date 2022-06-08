namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FA_O_Nas : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT [dbo].[Text] ([Key], [Value], [Title], [EditPermissions], [ViewPermissions], [OriginalUrl], [Icon]) VALUES (N'FA-o-nas', N'<h4>Jsme otevøen&aacute; skupina aktivn&iacute;ch lid&iacute;, kteø&iacute; nechtìj&iacute; sedìt doma.</h4>"
            + "<div>Tento web n&aacute;m <strong>umoòuje organizovat</strong> v&yacute;lety, sport, kulturu a dal&scaron;&iacute; rùzn&eacute; akce.Ve&scaron;ker&eacute; akce jsou <strong>volnì pø&iacute;stupn&eacute; pro kad&eacute;ho</strong>, kdo se zde zaregistruje a na danou akci pøihl&aacute;s&iacute;.</div>"
            + "<div>R&aacute;di uv&iacute;t&aacute;me nov&eacute; lidi, take pokud Tì zaujala kter&aacute; koliv z na&scaron;ich akc&iacute;, <strong> nev&aacute;hej a pøijï mezi n&aacute;s </strong>.</div>"
            + "<div>M&aacute;&scaron;-li vlastn& acute; n&aacute;pad, mùe&scaron; si vytvoøit svoji akci a nab&iacute;dnout ji tak ostatn&iacute;m uivatelùm tohoto webu.</div>"
            + "<div>Provoz tohoto webu a organizaci pøedplacen&yacute;ch akc&iacute; zaji&scaron;uje spolek<strong><a href = \"@Url.Content(\">Spoleèn&eacute; aktivity z.s.</a></strong></div>"
            + "<p>&nbsp;</p>"
            + "<div>V&iacute;ce informac&iacute; najde&scaron; v sekci N&aacute;povìda.Nebo n&aacute;m napi&scaron; na<strong> info(zavin&aacute;è)fungujemeaktivne.cz</strong></div>"
            + "', N'O nás', N'Council,Supervisor,Administrator', NULL, N'~/ Info', NULL)");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM [dbo].[Text] WHERE [Key]=N'FA-o-nas'");
        }
    }
}
