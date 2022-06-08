using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3F.Web.Helpers
{
    public static class EventHelper
    {
        public static MvcHtmlString ItemNeeded(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<span class=\"ItemNeeded\">*povinná položka</span>");
        }

        public static MvcHtmlString ItemNotNeeded(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<span class=\"ItemNotNeeded\">*nepovinná položka</span>");
        }

        public static MvcHtmlString ItemIfNeeded(this HtmlHelper helper, bool condition)
        {
            return (condition) ? ItemNeeded(helper) : ItemNotNeeded(helper);
        }
    }
}