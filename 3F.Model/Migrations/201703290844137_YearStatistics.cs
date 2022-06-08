namespace _3F.Model.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class YearStatistics : DbMigration
    {
        public override void Up()
        {
            // 2013
            Sql(@"INSERT [dbo].[Text] ([Key], [Value], [Title], [EditPermissions], [ViewPermissions], [OriginalUrl], [Icon]) VALUES (N'FA-2013',"
                + "N'<div><p>Malé statistické ohlédnutí za prvním rokem webu, co se povedlo a co se nepovedlo</p>"
                + "<div>Počet uskutečněných akcí: <b>180</b> akcí</div><div>Organizátorů s alespoň jednou akcí: <b>25</b></div>"
                + "<div>Uživatelů, kteří přišli alespoň na jednu akci: <b>107</b></div><br><p>"
                + "Akce s největší účastí:</p><ul>"
                + "<li><a href=\"/3F/akce/detail/Badmintonova-SupermegaLiga-Extra\">Badmintonová liga 1</a> - <b>21</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Zranice-na-oslavu-100-uzivatelu-Fungujeme\">Žranice na oslavu 100 uživatelů Fungujeme</a> - <b>20</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Kuzelky\">Kuželky</a> - <b>19</b> účastníků</li>"
                + "</ul><p></p><p>Nejpilnější organizátoři</p><ul><li>Bobo - <b>66</b> akcí</li><li>Nerothar - <b>17</b> akcí</li>"
                + "<li>pvymet - <b>14</b> akcí</li></ul><p></p><p>Nejpilnější návštěvníci akcí</p><ul>"
                + "<li>Manik - <b>78</b> akcí</li><li>Marťule - <b>69</b> akcí</li><li>Bobo a Pavel.24 - <b>62</b> akcí</li>"
                + "</ul><p></p>Na první rok to není špatné, nemyslíte? Tak ať se nám i vám stále tak daří."
                + "</div>"
                + "', N'Rok 2013', N'Council,Supervisor,Administrator', NULL, N'~/Info', NULL)");

            // 2014
            Sql(@"INSERT [dbo].[Text] ([Key], [Value], [Title], [EditPermissions], [ViewPermissions], [OriginalUrl], [Icon]) VALUES (N'FA-2014',"
                + "N'<div><p>Malé statistické ohlédnutí za druhým rokem webu, co se povedlo a co se nepovedlo</p>"
                + "<div>Počet uskutečněných akcí: <b>315</b> akcí</div><div>Organizátorů s alespoň jednou akcí: <b>51</b></div>"
                + "<div>Uživatelů, kteří přišli alespoň na jednu akci: <b>163</b></div><br><p>Akce s největší účastí:</p><ul>"
                + "<li><a href=\"/3F/akce/detail/Zranice-na-oslavu-1-roku-Fungujeme\">Žranice na oslavu 1 roku Fungujeme</a> - <b>33</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Prvni-Petraska-na-zimu-2014-3\">První Petráška na zimu 2013/2014</a> - <b>23</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Malesicky-park-s-petanque-a-grilovackou---kdo-si-hraje%2c-nezlobi\">Malešický park s petanque a grilovačkou</a> - <b>22</b> účastníků</li>"
                + "</ul><p></p><p>Nejpilnější organizátoři</p><ul><li>Bobo - <b>67</b> akcí</li><li>Nerothar - <b>60</b> akcí</li>"
                + "<li>Manik - <b>40</b> akcí</li></ul><p></p><p>Nejpilnější návštěvníci akcí</p><ul><li>Manik - <b>85</b> akcí</li>"
                + "<li>Nebul a Nerothar - <b>81</b> akcí</li><li>Marťule - <b>75</b> akcí</li></ul><p></p><p></p><div>Celkem na akcích: <b>1947</b> lidí</div>"
                + "<div>Nových uživatelů: <b>180</b></div><p></p><p>Akce s nejvíce komentáři:</p><ul>"
                + "<li><a href=\"/3F/akce/detail/Pohodove-bezky-II\">Pohodové běžky II.</a> - <b>147</b> komentářů</li>"
                + "<li><a href=\"/3F/akce/detail/Pojedme-na-hory\">Pojeďme na hory k Tygrovi do Hejnic</a> - <b>86</b> komentářů</li>"
                + "<li><a href=\"/3F/akce/detail/Navrh-na-pobyt-v-Jedovnici\">Víkend v Jedovnici</a> - <b>76</b> komentářů</li>"
                + "</ul><p></p></div>"
                + "', N'Rok 2014', N'Council,Supervisor,Administrator', NULL, N'~/Info', NULL)");

            // 2015
            Sql(@"INSERT [dbo].[Text] ([Key], [Value], [Title], [EditPermissions], [ViewPermissions], [OriginalUrl], [Icon]) VALUES (N'FA-2015',"
                + "N'<div><p>Malé statistické ohlédnutí za třetím rokem webu, co se povedlo a co se nepovedlo</p>"
                + "<div>Počet uskutečněných akcí: <b>316</b> akcí</div><div>Organizátorů s alespoň jednou akcí: <b>41</b></div>"
                + "<div>Uživatelů, kteří přišli alespoň na jednu akci: <b>141</b></div><br><p>Akce s největší účastí:</p><ul>"
                + "<li><a href=\"/3F/akce/detail/Vanocni-vecirek-FA\">Vánoční večírek FA</a> - <b>47</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Sri-Lanka-v-pivovaru\">Srí Lanka v pivovaru</a> - <b>39</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Tour-po-pivovarech-VI\">Tour po pivovarech VI.</a> - <b>37</b> účastníků</li>"
                + "</ul><p></p><p>Nejpilnější organizátoři</p><ul><li>Nerothar - <b>69</b> akcí</li><li>pong4 - <b>59</b> akcí</li>"
                + "<li>Manik - <b>50</b> akcí</li></ul><p></p><p>Nejpilnější návštěvníci akcí</p><ul><li>Nerothar - <b>112</b> akcí</li>"
                + "<li>Majk - <b>95</b> akcí</li><li>Manik - <b>80</b> akcí</li></ul><p></p><p></p><div>Celkem na akcích: <b>1821</b> lidí</div>"
                + "<div>Nových uživatelů: <b>106</b></div><p></p><p>Akce s nejvíce komentáři:</p><ul>"
                + "<li><a href=\"/3F/akce/detail/Vanocni-vecirek-FA\">Vánoční večírek FA</a> - <b>38</b> komentářů</li>"
                + "<li><a href=\"/3F/akce/detail/Volby-Rady-a-Dozorci-komise-a-Referendum-o-novych-stanovach\">Volby Rady a Dozorčí komise a Referendum o nových stanovách</a> - <b>30</b> komentářů</li>"
                + "<li><a href=\"/3F/akce/detail/Voderadske-buciny-a-rozhledna-Skalka\">Voděradské bučiny a rozhledna Skalka</a> - <b>27</b> komentářů</li>"
                + "</ul><p></p></div>"
                + "', N'Rok 2015', N'Council,Supervisor,Administrator', NULL, N'~/Info', NULL)");

            // 2016
            Sql(@"INSERT [dbo].[Text] ([Key], [Value], [Title], [EditPermissions], [ViewPermissions], [OriginalUrl], [Icon]) VALUES (N'FA-2016',"
                + "N'<div><p>Malé statistické ohlédnutí za již čtvrtým rokem webu, co se povedlo a co se nepovedlo</p>"
                + "<div>Počet uskutečněných akcí: <b>273</b> akcí</div><div>Organizátorů s alespoň jednou akcí: <b>38</b></div>"
                + "<div>Uživatelů, kteří přišli alespoň na jednu akci: <b>188</b></div><br><p>Akce s největší účastí:</p><ul>"
                + "<li><a href=\"/3F/akce/detail/Vanocni-vecirek-spolku\">Vánoční večírek spolku</a> - <b>38</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Tour-po-pivovarech-VIII\">Tour po pivovarech VIII.</a> - <b>24</b> účastníků</li>"
                + "<li><a href=\"/3F/akce/detail/Tour-po-pivovarech-IX\">Tour po pivovarech IX.</a> - <b>23</b> účastníků</li>"
                + "</ul><p></p><p>Nejpilnější organizátoři</p><ul><li>Nerothar - <b>63</b> akcí</li><li>Manik - <b>45</b> akcí</li>"
                + "<li>Tula - <b>22</b> akcí</li></ul><p></p><p>Nejpilnější návštěvníci akcí</p><ul>"
                + "<li>Míša - <b>81</b> akcí</li><li>Nerothar - <b>69</b> akcí</li><li>Kaabo - <b>65</b> akcí</li></ul><p></p><p>"
                + "</p><div>Celkem na akcích: <b>1991</b> lidí</div><div>Nových uživatelů: <b>153</b></div><p></p><p>Akce s nejvíce komentáři:</p><ul>"
                + "<li><a href=\"/3F/akce/detail/Ceskosaske-Svycarsko---prodlouzeny-vikend\">Českosaské Švýcarsko - prodloužený víkend</a> - <b>45</b> komentářů</li>"
                + "<li><a href=\"/3F/akce/detail/Srpnova-Vltava-4\">Srpnová Vltava 4</a> - <b>44</b> komentářů</li>"
                + "<li><a href=\"/3F/akce/detail/Prvni-Petraska-v-roce-2016\">První Petráška v roce 2016</a> - <b>43</b> komentářů</li>"
                + "</ul><p></p></div>"
                + "', N'Rok 2016', N'Council,Supervisor,Administrator', NULL, N'~/Info', NULL)");

            // Authors
            Sql(@"INSERT [dbo].[Text] ([Key], [Value], [Title], [EditPermissions], [ViewPermissions], [OriginalUrl], [Icon]) VALUES (N'FA-Authors',"
                + "N'<div><h4>Autor</h4><p>Autorem webové aplikace je Dušan Ryšavý. V případě potřeby jej lze kontaktovat na adrese "
                + "<a href=\"mailto:mail@dusanrysavy.cz\">mail@dusanrysavy.cz</a>.</p><h4>Aplikace</h4><p>"
                + "</p><div>Aplikace byla vytvořena ve chvílích odpočinkových pro radost moji a užitek a požitek ostatních.</div>"
                + "<div>Nechť slouží a vzkvétá :-)</div>"
                + "<div>Děkuji testerovi Manikovi za množství nalezených chyb a jeho neutuchající touhu hledat další. A i za další podnětné návrhy k "
                + "zlepšení uživatelského komfortu.</div>"
                + "<p></p></div>"
                + "', N'Autoři', N'Council,Supervisor,Administrator', NULL, N'~/Info', NULL)");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM [dbo].[Text] WHERE [Key]=N'FA-2013'");
            Sql("DELETE FROM [dbo].[Text] WHERE [Key]=N'FA-2014'");
            Sql("DELETE FROM [dbo].[Text] WHERE [Key]=N'FA-2015'");
            Sql("DELETE FROM [dbo].[Text] WHERE [Key]=N'FA-2016'");
            Sql("DELETE FROM [dbo].[Text] WHERE [Key]=N'FA-Authors'");
        }
    }
}
