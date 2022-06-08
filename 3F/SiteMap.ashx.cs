using System;
using System.Web;
using _3F.Model;
using _3F.Model.Model;

namespace _3F.Web
{
    /// <summary>
    /// Summary description for SiteMap
    /// </summary>
    public class SiteMap : IHttpHandler
    {
        private IRepository repository;

        public void ProcessRequest(HttpContext context)
        {
            repository = new Repository();

            context.Response.ContentType = "text/xml";
            context.Response.Write(string.Format("<urlset xmlns=\"{0}\">", "http://www.sitemaps.org/schemas/sitemap/0.9"));
            var url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            #region Mapa webu z website
            WriteToResponse(context.Response, url.ToString(), Info.CentralEuropeNow, ChangeFreq.daily, 1);
            WriteToResponse(context.Response, url + "/Home/Uplynule", Info.CentralEuropeNow, ChangeFreq.daily, 1);
            WriteToResponse(context.Response, url + "/Fotky", Info.CentralEuropeNow, ChangeFreq.daily, 1);
            WriteToResponse(context.Response, url + "/Info", Info.CentralEuropeNow, ChangeFreq.daily, 1);
            WriteToResponse(context.Response, url + "/Zapis", Info.CentralEuropeNow, ChangeFreq.daily, 1);
            #endregion

            #region Výpis akcí
            foreach (var action in repository.Where<Event>(ev => ev.StopDateTime < Info.CentralEuropeNow && ev.State == EventStateEnum.Active && ev.EventType != EventTypeEnum.Soukroma))
            {
                WriteToResponse(context.Response, url + "/Akce/Detail/" + action.HtmlName, action.StartDateTime, ChangeFreq.monthly, 1d);
            }

            foreach (var action in repository.Where<Event>(ev => ev.StopDateTime >= Info.CentralEuropeNow && ev.State == EventStateEnum.Active && ev.EventType != EventTypeEnum.Soukroma))
            {
                WriteToResponse(context.Response, url + "/Akce/Detail/" + action.HtmlName, Info.CentralEuropeNow, ChangeFreq.daily, 1d);
            }
            #endregion

            #region Výpis diskuzí
            foreach (var item in repository.Where<Discussion>(d => d.IsAlone))
            {
                WriteToResponse(context.Response, url + "/Diskuze/Detail/" + item.HtmlName, Info.CentralEuropeNow, ChangeFreq.daily, 1d);
            }
            #endregion

            #region Výpis zápisků
            foreach (var item in repository.All<EventSummary>())
            {
                WriteToResponse(context.Response, url + "/Zapis/Detail/" + item.Event.HtmlName, item.Event.StopDateTime, ChangeFreq.monthly, 1d);
            }
            #endregion


            context.Response.Write("</urlset>");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void WriteToResponse(HttpResponse response, string loc, DateTime lastmod, ChangeFreq changefreq, double priority)
        {
            response.Write("<url>");
            response.Write(string.Format("<loc>{0}</loc>", loc));
            response.Write(string.Format("<lastmod>{0}</lastmod>", lastmod.ToString("yyyy-MM-dd")));
            response.Write(string.Format("<changefreq>{0}</changefreq>", changefreq));
            response.Write(string.Format("<priority>{0}</priority>", priority.ToString("0.00").Replace(",", ".")));
            response.Write("</url>");
        }

        private enum ChangeFreq
        {
            always,
            hourly,
            daily,
            weekly,
            monthly,
            yearly,
            never
        }
    }
}