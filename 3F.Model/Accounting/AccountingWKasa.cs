using _3F.Log;
using _3F.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _3F.Model.Accounting
{
    public class AccountingWKasa : IAccounting
    {
        private string Salt = "salt";
        private string DataUrlFormat = "https://platby.alwaysdata.net/ws/udaje.xml/{0}/{1}";
        private string MoveUrlFormat = "https://platby.alwaysdata.net/ws/pohyb.xml/{0}/{1}/{2}/{3}/{4}/{5}";
        private string NewUserUrlFormat = "https://platby.alwaysdata.net/ws/novy.xml/{0}/{1}/{2}";
        private string GetCostsUrlFormat = "https://platby.alwaysdata.net/ws/naklady.xml/{0}/{1}";
        private ILogger logger;

        public AccountingWKasa(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<AccountData> GetData(int vs)
        {
            return await this.GetData(vs.ToString());
        }

        public async Task<AccountData> GetData(string email)
        {
            try
            {
                string query = string.Format(DataUrlFormat, GetMD5Hash(email), email);
                string xml = string.Empty;

                logger.LogDebug(string.Format("Provádění dotazu {0} na VS nebo email {1}", query, email), "Accounting.GetData");
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (WebClient wc = new WebClient())
                {
                    xml = wc.DownloadString(new Uri(query)); //stahnout soubor
                }
                logger.LogDebug(string.Format("Stažení odpovědi požadavku na VS: {0} doběhlo", email), "Accountig.GetData");

                return ParseDataFromXML(xml);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Dotaz na VS: {0} skončil s chybou {1}", email, ex.Message), "Accounting.GetData");
                return AccountData.EmptyData;
            }
        }

        public async Task<MoveResult> MakeMove(int vs, double amount, string email, int event_vs, string note, int id_Payment)
        {
            if (!Properties.Settings.Default.Accounting)
                return MoveResult.EmptyResult;

            note = note.TakeSafetely(100);
            try
            {
                string query = string.Format(MoveUrlFormat,
                    GetMD5Hash(string.Format("{0}{1}", vs, amount)),
                    vs,
                    amount,
                    email,
                    event_vs,
                    RemoveDiakritics(note));

                query = Uri.EscapeUriString(query);

                logger.LogDebug(string.Format("Provádění požadavku {4} platby k akci {3} s vs {0} na částku {1} s poznámkou {2}, query {5}",
                    vs, amount, note, event_vs, id_Payment, query),
                    "Accounting.MakeMove");

                string xml = "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (WebClient wc = new WebClient())
                {
                    xml = wc.DownloadString(query); //stahnout soubor
                }
                logger.LogDebug(string.Format("Stažení odpovědi požadavku {0} doběhlo", id_Payment), "Accountig.MakeMove");
                MoveResult move = ParseMoveFromXML(xml);

                logger.LogDebug(string.Format("Výsledek požadavku {0} je {1}", id_Payment, move.Paid), "Accounting.MakeMove");

                return move;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Při provádění požadavku k akci {4} na variabilní symbol {0} na částku {1} s poznámkou {2} se vyskytla chyba {3}",
                    vs, amount, note, ex.Message, event_vs), "Accounting.MakeMove");
                return MoveResult.EmptyResult;
            }
        }

        public async Task<NewUserResult> GetNewUserSymbol(string email, string userName)
        {
            try
            {
                string query = string.Format(NewUserUrlFormat,
                    GetMD5Hash(email),
                    email,
                    GetBase64String(userName));

                query = Uri.EscapeUriString(query);

                logger.LogDebug(string.Format("Zjišťování VS {2} pro uživatele '{0}' s emailem: {1}",
                    userName, email, query),
                    "Accounting.GetNewUserSymbol");

                string xml = "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (WebClient wc = new WebClient())
                {
                    xml = wc.DownloadString(query); //stahnout soubor
                }
                logger.LogDebug(string.Format("Dotaz na uživatele '{0}' doběhl", userName), "Accountig.GetNewUserSymbol");
                NewUserResult result = ParseUserFromXML(xml);

                logger.LogDebug(string.Format("Výsledek požadavku Vs na uživatele '{0}' je {1}, problem '{2}'", userName, result.Symbol, result.Problem), "Accounting.GetNewUserSymbol");

                if (!string.IsNullOrEmpty(result.Problem))
                    return NewUserResult.EmptyResult;

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Dotaz na uživatele '{0}' skončil s chybou {1}",
                    userName, ex.Message), "Accounting.GetNewUserSymbol");
                return NewUserResult.EmptyResult;
            }
        }

        public async Task<List<Cost>> GetCosts(int eventNumber)
        {
            try
            {
                string query = string.Format(GetCostsUrlFormat, GetMD5Hash(eventNumber), eventNumber);
                string xml = string.Empty;

                logger.LogDebug(string.Format("Provádění dotazu {0} na náklady akce {1}", query, eventNumber), "Accounting.GetCosts");

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    xml = wc.DownloadString(new Uri(query)); //stahnout soubor
                }
                logger.LogDebug(string.Format("Stažení odpovědi požadavku na náklady akce: {0} doběhlo", eventNumber), "Accountig.GetCosts");

                return ParseCostsFromXML(xml);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Dotaz na náklady akce: {0} skončil s chybou {1}", eventNumber, ex.Message), "Accounting.GetCosts");
                return new List<Cost>();
            }
        }

        private string GetMD5Hash(int number)
        {
            return GetMD5Hash(number.ToString());
        }

        private string GetMD5Hash(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Salt + text));
            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private string RemoveDiakritics(string text)
        {
            string stringFormD = text.Normalize(NormalizationForm.FormD);
            StringBuilder retVal = new StringBuilder();
            for (int index = 0; index < stringFormD.Length; index++)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(stringFormD[index]) != UnicodeCategory.NonSpacingMark)
                    retVal.Append(stringFormD[index]);
            }

            string result = retVal.ToString().Normalize(NormalizationForm.FormC);
            result = result.Replace(",", "-").Replace("&", "-").Replace("<", "-").Replace(">", "-");
            result = result.Replace(".", "-").Replace("!", "").Replace("|", "-").Replace("'", "");
            result = result.Replace("?", "").Replace(":", "-").Replace("*", "-").Replace(@"\", "-");

            return result.Replace("(", "").Replace(")", "");
        }

        private string GetBase64String(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        private AccountData ParseDataFromXML(string xml)
        {
            try
            {
                /*
                <document>
                    <zaloha>619.00</zaloha>
                    <vs>269</vs>
                    <email>sqwert@seznam.cz</email>
                </document>
                */ 
                XElement xdoc = XDocument.Parse(xml).Element("document");
                return new AccountData(xdoc.Element("email").Value, xdoc.Element("vs").Value, xdoc.Element("zaloha").Value, true);
            }
            catch
            {
                logger.LogDebug(string.Format("Nelze rozparsovat odpověď {0}", xml), "AccountData.ParseDataFromXML");
                return new AccountData(string.Empty, 0, 0, true);
            }
        }

        private MoveResult ParseMoveFromXML(string xml)
        {
            try
            {
                /*
                <document>
                    <provedeno>True</provedeno>
                    <castka>632.0</castka>
                </document>
                */
                XElement xdoc = XDocument.Parse(xml).Element("document");
                return new MoveResult(Convert.ToBoolean(xdoc.Element("provedeno").Value), true);
            }
            catch
            {
                logger.LogDebug(string.Format("Nelze rozparsovat odpověď {0}", xml), "AccountData.ParseMoveFromXML");
                return new MoveResult(false, true);
            }
        }

        private NewUserResult ParseUserFromXML(string xml)
        {
            try
            {
                /*
                <document>
                    <problem>True</problem>
                    <vs>632.0</vs>
                </document>
                */
                XElement xdoc = XDocument.Parse(xml).Element("document");
                return new NewUserResult(xdoc.Element("problem").Value, Convert.ToInt32(xdoc.Element("vs").Value));
            }
            catch
            {
                logger.LogDebug(string.Format("Nelze rozparsovat odpověď {0}", xml), "AccountData.ParseUserFromXML");
                return NewUserResult.EmptyResult;
            }
        }

        private List<Cost> ParseCostsFromXML(string xml)
        {
            var result = new List<Cost>();
            try
            {
                /*
                    <tbl>
                        <row>
                            <castka>550.00</castka>
                            <popis>Berounka_z_Liblina_do_Zvikovce</popis>
                        </row>
                        <row>
                            <castka>550.00</castka>
                            <popis>Berounka_z_Liblina_do_Zvikovce</popis>
                        </row>
                    </tbl>
                */
                XElement xdoc = XDocument.Parse(xml).Element("tbl");
                var rows = xdoc.Elements("row");
                foreach (var row in rows)
                {
                    result.Add(new Cost
                    {
                        Amount = ConvertToDecimal(row.Element("castka").Value),
                        Description = row.Element("popis").Value
                    });
                }
            }
            catch
            {
                logger.LogDebug(string.Format("Nelze rozparsovat odpověď {0}", xml), "AccountData.ParseCostsFromXML");
            }

            return result;
        }

        private decimal ConvertToDecimal(string value)
        {
            var separator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            if (separator == ",")
                value = value.Replace(".", ",");

            return Convert.ToDecimal(value);
        }
    }
}
