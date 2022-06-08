using System.Web.Optimization;
using _3F.Model;

namespace _3F
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jscript").Include(
                      "~/Scripts/jquery-1.10.2.min.js",
                      "~/Scripts/jquery.mobile.custom.min.js",
                      "~/Scripts/jquery-migrate.min.js",
                      "~/Scripts/jquery-ui.min.js",
                      "~/Scripts/plugins/jquery_ui_touch_punch/jquery.ui.touch-punch.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/plugins/retina/retina.js",
                      "~/Scripts/modernizr-2.6.2.js",
                      "~/Scripts/theme.js",
                      "~/Scripts/plugins/bootbox/bootbox.min.js",
                      "~/Scripts/plugins/mustache/mustache.js",
                      "~/Scripts/jquery.twbsPagination.js",
                      "~/Scripts/jquery.unobtrusive-ajax.js",
                      "~/Scripts/3F.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));
            
            //if(Info.CentralEuropeNow.Day == 1 && Info.CentralEuropeNow.Month == 1)
            //{
            //    bundles.Add(new StyleBundle("~/Content/css2")
            //        .Include(
            //          "~/Content/14-light-theme.css",
            //          "~/Content/14-site.css",
            //          "~/Content/14-theme-colors.css"));
            //}
            //else
            //{
                bundles.Add(new StyleBundle("~/Content/css2").Include(
                      "~/Content/light-theme.css",
                      "~/Content/site.css",
                      "~/Content/theme-colors.css"));
            //}

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                      "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                    "~/Scripts/plugins/datatables/jquery.dataTables.min.js",
                    "~/Scripts/plugins/datatables/jquery.dataTables.columnFilter.min.js",
                    "~/Scripts/plugins/datatables/czDatetimeSort.js",
                    "~/Scripts/plugins/datatables/dataTables.overrides.js"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}