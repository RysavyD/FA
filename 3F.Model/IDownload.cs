using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using _3F.Log;
using _3F.Model.Extensions;
using _3F.Model.Model;
using HtmlAgilityPack;

namespace _3F.Model
{

    public interface IDownload
    {
        void Download(int startID, bool overwrite, bool singleItem, int? pageStart);
    }

    public class TouristStampDownload : IDownload
    {
        IRepository repository;
        ILogger logger;

        public TouristStampDownload(IRepository repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public void Download(int startID, bool overwrite, bool singleItem, int? pageStart)
        {

            WebRequest wr = WebRequest.Create("http://www.turisticke-znamky.cz/export.php?item=1&type=csv");

            var reader = new StreamReader(wr.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8);
            reader.ReadLine(); //hlavicka me nazajima

            Dictionary<int, string> items = new Dictionary<int, string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] array = line.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                int ID = 0;
                int.TryParse(array[0], out ID);
                string Name = array[1];
                Name = Name.StartsWith("\"") ? Name.Substring(1, Name.Length - 2) : Name;
                items.Add(ID, Name);
            }

            foreach(var item in items)
            { 
                if ((item.Key >= startID && !singleItem) || (item.Key == startID && singleItem))
                {
                    try
                    {
                        bool newItem = false;
                        var entity = repository.One<TouristStamp>(e => e.ItemNumber == item.Key);
                        if (entity == null)
                        {
                            entity = new TouristStamp() { ItemNumber = item.Key };
                            newItem = true;
                        }
                        else if (!overwrite)
                            continue;

                        string requestString = string.Format("http://www.turisticke-znamky.cz/znamky/{0}-c{1}", item.Value.RemoveDiakritics().Replace("---", "-"), item.Key);

                        WebClient wc = new WebClient();
                        string webPage = wc.DownloadString(requestString);

                        byte[] bytes = System.Text.Encoding.Default.GetBytes(webPage);
                        bytes = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, bytes);
                        webPage = System.Text.Encoding.Default.GetString(bytes);

                        HtmlDocument docu = new HtmlDocument();
                        docu.LoadHtml(webPage);

                        var columnMiddleText = docu.GetElementbyId("columnMiddleText");
                        var title = columnMiddleText.Descendants("h1").First();
                        entity.Name = title.InnerText.Substring(title.InnerText.IndexOf("- ") + 2);

                        var detailLeftIn = docu.GetElementbyId("detailLeftIn");
                        if (detailLeftIn == null) continue;

                        var content = detailLeftIn.ChildNodes.Skip(1).First();

                        entity.Description = WebUtility.HtmlDecode(content.InnerText);

                        var geo = docu.DocumentNode.Descendants().SingleOrDefault(node => node.Attributes.Any(a => a.Name == "class" && a.Value == "geo"));
                        var latitude = geo.FirstChild.Attributes["title"].Value;
                        var longitude = geo.LastChild.Attributes["title"].Value;
                        entity.Position = DbGeography.FromText(string.Format("POINT({0} {1})", longitude, latitude));

                        var imageClass = docu.DocumentNode.Descendants().FirstOrDefault(node => node.Attributes.Any(a => a.Name == "class" && a.Value == "lbTdIn"));
                        if (imageClass != null)
                        {
                            var imageLink = imageClass.ChildNodes.First().Attributes["href"].Value;
                            entity.ImageUrl = string.Format("http://www.turisticke-znamky.cz/{0}", imageLink);
                        }

                        if (newItem)
                            repository.Add(entity);

                        repository.Save();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(string.Format("Při stahvoání infa o známce s číslem {0} nastala chyba.", item.Key), "TouristStampDownload.Download");
                        logger.LogException(ex, "TouristStampDownload.Download");
                    }
                }
            }
        }
    }

    public class TouristCardDownload : IDownload
    {
        IRepository repository;
        ILogger logger;

        public TouristCardDownload(IRepository repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public void Download(int startID, bool overwrite, bool singleItem, int? pageStart)
        {
            int page = pageStart ?? 1;
            var cards = CarFinded(page);

            while (cards.Count > 0)
            {
                foreach (var card in cards)
                {
                    if ((card.Id >= startID && !singleItem) || (card.Id == startID && singleItem))
                    {
                        var entity = repository.One<TouristCard>(c => c.ItemNumber == card.Id);
                        if (entity != null && !overwrite)
                            continue;

                        if (!string.IsNullOrWhiteSpace(card.Position))
                        {
                            if (entity != null)
                            {
                                entity.Name = card.Name;
                                entity.Description = card.Description;
                                entity.ImageUrl = card.ImageUrl;
                                entity.ItemNumber = card.Id;
                                entity.Position = DbGeography.FromText(String.Format("POINT({0})", card.Position));
                            }
                            else
                            {
                                entity = new TouristCard()
                                {
                                    Description = card.Description,
                                    ImageUrl = card.ImageUrl,
                                    ItemNumber = card.Id,
                                    Name = card.Name,
                                    Position = DbGeography.FromText(String.Format("POINT({0})", card.Position)),

                                };

                                repository.Add(entity);
                            }

                            repository.Save();
                        }

                        if (singleItem)
                            return;
                    }
                }

                page++;
                cards = CarFinded(page);
            }
        }

        private List<CardInfo> CarFinded(int page)
        {
            var result = new List<CardInfo>();

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var url = $"https://cs.wander-book.com/turistika.htm?stranka={page}&q=&druh[]=tv&oblast[]=1&zeme[]=1#fp";
                string webPage = wc.DownloadString(url);

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(webPage);
                var elements = document.DocumentNode.SelectNodes("//*/div[@class='card-header-padding']");
                string IDstring = "";
                if (elements != null && elements.Count() > 0)
                {
                    foreach (var element in elements)
                    {
                        try
                        {
                            var linkElement = element.FirstChild;
                            IDstring = linkElement.InnerText.Split('-')[1];
                            if (IDstring.Contains("G"))
                                continue;

                            int ID = Convert.ToInt32(IDstring);
                            string name = linkElement.Attributes["title"].Value;
                            string link = "https://cs.wander-book.com" + linkElement.Attributes["href"].Value;

                            var detailPage = wc.DownloadString(link);
                            HtmlDocument detailPageDocument = new HtmlDocument();
                            detailPageDocument.LoadHtml(detailPage);

                            var detailElement = detailPageDocument.DocumentNode.SelectSingleNode("//*/p[@class='detail']");
                            var description = detailElement.InnerText;

                            var imageElement = detailPageDocument.DocumentNode.SelectSingleNode("//*/img[@class='img-shadow imageItem']");
                            var imageUrl = imageElement.Attributes["src"].Value;

                            //var positionElement = detailPageDocument.DocumentNode.SelectSingleNode("/html/body/div[1]/div[3]/div/div[5]/div/div[2]/ul/li[4]/div[3]");
                            var positionElement = detailPageDocument.DocumentNode.SelectSingleNode("//*/div[@class='circle gps']");
                            var position = GetPosition(positionElement.NextSibling.NextSibling.InnerText);

                            result.Add(new CardInfo()
                            {
                                Id = ID,
                                Name = name,
                                Description = description,
                                ImageUrl = imageUrl,
                                Link = link,
                                Position = position
                            });

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{IDstring} - {ex.Message}");
                        }
                    }
                }
            }

            return result;
        }

        private string GetPosition(string gpsText)
        {
            try
            {
                var gps = gpsText.Trim();
                // 50°43'57"N, 14°59'5"E
                var array = gps.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                return string.Format("{0} {1}", GetGeographyLength(array[1]), GetGeographyLength(array[0])).Replace(",", ".");
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetGeographyLength(string source)
        {
            int index1 = source.IndexOf("°");
            int index2 = source.IndexOf("'");
            int index3 = source.IndexOf("\"");

            if (index1 < 0 || index2 < 0 || index3 < 0)
                return string.Empty;

            int hours = Convert.ToInt32(source.Substring(0, index1));
            int minutes = Convert.ToInt32(source.Substring(index1 + 1, index2 - index1 - 1));
            double seconds = ToDouble(source.Substring(index2 + 1, index3 - index2 - 1));

            double result = hours + minutes / 60d + seconds / 3600;

            return result.ToString();
        }

        private double ToDouble(string text)
        {
            double result;
            if (double.TryParse(text.Replace(",", "."), out result))
                return result;

            if (double.TryParse(text.Replace(".", ","), out result))
                return result;

            return 0d;
        }

        private class CardInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Position { get; set; }
            public string ImageUrl { get; set; }
            public string Link { get; set; }
        }
    }
}