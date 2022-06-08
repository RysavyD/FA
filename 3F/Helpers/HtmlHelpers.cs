using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using _3F.Model.Model;
using _3F.Web.Models;

namespace _3F.Web.Helpers
{
    public static class HtmlHelpers
    {
        #region Html block
        private class HtmlBlock : IDisposable
        {
            private const string scriptsKey = "htmlBlock";
            public static List<string> htmlBlocks
            {
                get
                {
                    if (HttpContext.Current.Items[scriptsKey] == null)
                        HttpContext.Current.Items[scriptsKey] = new List<string>();
                    return (List<string>)HttpContext.Current.Items[scriptsKey];
                }
            }

            WebViewPage webPageBase;

            public HtmlBlock(WebViewPage webPageBase)
            {
                this.webPageBase = webPageBase;
                this.webPageBase.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                htmlBlocks.Add(((StringWriter)this.webPageBase.OutputStack.Pop()).ToString());
            }
        }

        public static IDisposable BeginHtmlBlocks(this HtmlHelper helper)
        {
            return new HtmlBlock((WebViewPage)helper.ViewDataContainer);
        }

        public static MvcHtmlString HtmlBlocks(this HtmlHelper helper)
        {
            return MvcHtmlString.Create(string.Join(Environment.NewLine, HtmlBlock.htmlBlocks.Select(s => s.ToString())));
        }
        #endregion

        public static MvcHtmlString Decode(this HtmlHelper helper, string text)
        {
            return new MvcHtmlString(WebUtility.HtmlDecode(text));
        }

        public static MvcHtmlString MenuItem(this HtmlHelper helper, string text, string controller, string action)
        {
            return helper.MenuItem(text, controller, action, string.Empty);
        }

        public static MvcHtmlString MenuItem(this HtmlHelper helper, string text, string controller, string action, string icon, string id = "")
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext).Action(action, controller, new { id = string.Empty });
            return new MvcHtmlString(string.Format("<li class=''>"
                    + "<a href='{2}'>"
                        + "<i class='{1}'></i>"
                        + "<span id='{3}'>{0}</span>"
                    + "</a>"
                + "</li>", text, icon, url, id));
        }

        public static MvcHtmlString PictureItem(this HtmlHelper helper, string smallPictureUrl, string bigPictureUrl)
        {
            return new MvcHtmlString(string.Format("<li>"
                            + "<div class=\"picture\">"
                                + "<a data-lightbox=\"flatty\" href=\"{1}\">"
                                    + "<img src=\"{0}\">"
                                + "</a>"
                            + "</div>"
                        + "</li>", smallPictureUrl, bigPictureUrl));
        }

        public static MvcHtmlString ShowCheck(this HtmlHelper helper, string text, bool value)
        {
            return new MvcHtmlString(string.Format("<li><i class=\"{0}\"></i> {1}</li>",
                (value) ? "icon-check" : "icon-check-empty",
                text));
        }

        public static MvcHtmlString GetSexIcon(this HtmlHelper helper, SexEnum sex)
        {
            if (sex == SexEnum.Male)
                return new MvcHtmlString("<i class=\"icon-male\"></i>");

            if (sex == SexEnum.Female)
                return new MvcHtmlString("<i class=\"icon-female\"></i>");

            return new MvcHtmlString(string.Empty);
        }

        public static MvcHtmlString FormSubmitWaitDialog(this HtmlHelper helper)
        {

            return new MvcHtmlString("<script>" +
                    "$(\"form\").submit(function() {" +
                    "ShowWaitDialog();" +
                    "});" +
                    "</script>");
        }

        public static MvcHtmlString TextLinkToProfil(this HtmlHelper helper, User user)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext).Action("Detail", "Profil", new { id = user.htmlName });
            return new MvcHtmlString(string.Format("<a href='{0}'>{1}</a>", url, user.name));
        }

        public static MvcHtmlString RenderModalPlaceHolder(this HtmlHelper helper)
        {
            return new MvcHtmlString("<div id=\"myModal\" class=\"modal\"><div class=\"modal-dialog\">"
                + "<div class=\"modal-content\"><div id=\"myModalContent\"></div></div></div></div>");
        }

        public static MvcHtmlString DiscussionScripts(this HtmlHelper helper, int discussionId)
        {
            var url  = new UrlHelper(helper.ViewContext.RequestContext);
            return new MvcHtmlString("<script type=\"text/javascript\">"
                + "$(document).ready(function() {"
                + "MakeGetCall(\"#discussion\", '" + url.Content("~/Api/Diskuze/Get/" + discussionId) 
                + "', 1, CreatePage);"
                + "$(\"#sendNewMessage\").click(function() {"                
                + "var NewDiscussionMessage = { id: "+ discussionId +", message: $(\"#message_body\").val() };"
                + "MakePostCall('" + url.Content("~/Api/Diskuze/Post")
                + "', NewDiscussionMessage); }); });"
                + "function CreatePage(data) { "
                + "GeneratePagination(\"#discussionPagination\", GetData, data.TotalItems); }"
                + "function GetData(page) {"
                + "MakeGetCall(\"#discussion\", '"  + url.Content("~/Api/Diskuze/Get/" + discussionId)
                + "' , page); } </script>");
        }

        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            var template = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(template, actionName, controllerName, routeValues, ajaxOptions);
            return MvcHtmlString.Create(lnk.ToString().Replace(template, linkText));
        }

        public static MvcHtmlString ChekboxWithName(this HtmlHelper helper, string name, SelectListItem item)
        {
            var html = $"<input type=\"checkbox\" value=\"{item.Value}\" name=\"{name}\" ";
            html += item.Selected ? "checked=\"checked\"" : "";
            html += " />";

            return new MvcHtmlString(html);
        }

        public static MvcHtmlString EventDetailLink(this HtmlHelper html, string text, string htmlName)
        {
            return html.ActionLink(text, "Detail", "Akce", new {id = htmlName}, new { });
        }

        public static string GetUserHtmlName(this HtmlHelper html)
        {
            return html.ViewContext.HttpContext.User.Identity.IsAuthenticated
                ? ((ClaimsIdentity)html.ViewContext.HttpContext.User.Identity)?.FindFirst("HtmlName")?.Value ?? "Unknown"
                : "Unknown";
        }
    }
}