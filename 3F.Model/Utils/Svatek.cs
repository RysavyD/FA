using System;

namespace _3F.Model.Utils
{
    public class Svatek
    {
        public static string GetSvatek(DateTime date)
        {
            var yearArray = new string[][] {
                new string[] {"Nový rok","Karina","Radmila","Diana","Dalimil","Tři králové","Vilma","Čestmír","Vladan","Břetislav","Bohdana","Pravoslav","Edita","Radovan","Alice","Ctirad","Drahoslav","Vladislav","Doubravka","Ilona","Běla","Slavomír","Zdeněk","Milena","Miloš","Zora","Ingrid","Otýlie","Zdislava","Robin","Marika"},
                new string[] {"Hynek","Nela","Blažej","Jarmila","Dobromila","Vanda","Veronika","Milada","Apolena","Mojmír","Božena","Slavěna","Věnceslav","Valentýn","Jiřina","Ljuba","Miloslava","Gizela","Patrik","Oldřich","Lenka","Petr","Svatopluk","Matěj","Liliana","Dorota","Alexandr","Lumír","Horymír"},
                new string[] {"Bedřich","Anežka","Kamil","Stela","Kazimir","Miroslav","Tomáš","Gabriela","Františka","Viktorie","Anděla","Řehoř","Růžena","Rút a Matylda","Ida","Elena a herbert","Vlastimil","Eduard","Josef","Světlana","Radek","Leona","Ivona","Gabriel","Marián","Emanuel","Dita","Soňa","Taťána","Arnošt","Kvido"},
                new string[] {"Hugo","Erika","Richard","Ivana","Miroslava","Vendula","Heřman a Hermína","Ema","Dušan","Darja","Izabela","Julius","Aleš","Vincenc","Anastázie","Irena","Rudolf","Valérie","Rostislav","Marcela","Alexandra","Evženie","Vojtěch","Jiří","Marek","Oto","Jaroslav","Vlastislav","Robert","Blahoslav"},
                new string[] {"Svátek práce","Zikmund","Alexej","Květoslav","Klaudie","Radoslav","Stanislav","","Ctibor","Blažena","Svatava","Pankrác","Servác","Bonifác","Žofie","Přemysl","Aneta","Nataša","Ivo","Zbyšek","Monika","Emil","Vladimír","Jana","Viola","Filip","Valdemar","Vilém","Maxmilián","Ferdinand","Kamila"},
                new string[] {"Laura","Jarmil","Tamara","Dalibor","Dobroslav","Norbert","Iveta a Slavoj","Medard","Stanislava","Gita","Bruno","Antonie","Antonín","Roland","Vít","Zbyněk","Adolf","Milan","Leoš","Květa","Alois","Pavla","Zdeňka","Jan","Ivan","Adriana","Ladislav","Lubomír","Petr a Pavel","Šárka"},
                new string[] {"Jaroslava","Patricie","Radomír","Prokop","Cyril a Metoděj"," ","Bohuslava","Nora","Drahoslava","Libuše a Amálie","Olga","Bořek","Markéta","Karolína","Jindřich","Luboš","Martina","Drahomíra","Čeněk","Ilja","Vítězslav","Magdaléna","Libor","Kristýna","Jakub","Anna","Věroslav","Viktor","Marta","Bořivoj","Ignác"},
                new string[] {"Oskar","Gustav","Miluše","Dominik","Kristian","Oldřiška","Lada","Soběslav","Roman","Vavřinec","Zuzana","Klára","Alena","Alan","Hana","Jáchym","Petra","Helena","Ludvík","Bernard","Johana","Bohuslav","Sandra","Bartoloměj","Radim","Luděk","Otakar","Augustýn","Evelína","Vladěna","Pavlína"},
                new string[] {"Linda a Samuel","Adéla","Bronislav","Jindřiška","Boris","Boleslav","Regína","Mariana","Daniela","Irma","Denisa","Marie","Lubor","Radka","Jolana","Ludmila","Naděžda","Kryštof","Zita","Oleg","Matouš","Darina","Berta","Jaromír","Zlata","Andrea","Jonáš","Václav","Michal","Jeroným"},
                new string[] {"Igor","Olivie a Oliver","Bohumil","František","Eliška","Hanuš","Justýna","Věra","Štefan a Sára","Marina","Andrej","Marcel","Renáta","Agáta","Tereza","Havel","Hedvika","Lukáš","Michaela","Vendelín","Brigita","Sabina","Teodor","Nina","Beáta","Erik","Šarlota a Zoe"," ","Silvie","Tadeáš","Štěpánka"},
                new string[] {"Felix","Tobiáš","Hubert","Karel","Miriam","Liběna","Saskie","Bohumír","Bohdan","Evžen","Martin","Benedikt","Tibor","Sáva","Leopold","Otmar","Mahulena","Romana","Alžběta","Nikola","Albert","Cecílie","Klement","Emílie","Kateřina","Artur","Xenie","René","Zina","Ondřej"},
                new string[] {"Iva","Blanka","Svatoslav","Barbora","Jitka","Mikuláš","Ambrož a Benjamín","Květoslava","Vratislav","Julie","Dana","Simona","Lucie","Lýdie","Radana a Radan","Albína","Daniel","Miloslav","Ester","Dagmar","Natálie","Šimon","Vlasta","Adam a Eva"," ","Štěpán","Žaneta","Bohumila","Judita","David","Silvestr"},
            };
            return yearArray[date.Month - 1][date.Day - 1];
        }

        public static string DnesniSvatek()
        {
            return GetSvatek(Info.CentralEuropeNow);
        }
    }
}
