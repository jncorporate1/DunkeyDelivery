using System.Web;
using System.Web.Optimization;

namespace BasketWebPanel
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Content/site.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome/css/font-awesome.min.css",
                      //"~/Content/font-awesome/font-awesome.css",
                      "~/Content/nprogress/nprogress.css",
                      "~/Content/iCheck/skins/flat/green.css",
                      "~/Content/bootstrap-progressbar/css/bootstrap-progressbar-3.3.4.min.css",
                      "~/Content/jqvmap/dist/jqvmap.min.css",
                      "~/Content/bootstrap-daterangepicker/daterangepicker.css",
                      //"~/Content/build/css/custom.min.css",
                      "~/Content/animate.css/animate.css",
                      "~/Content/admin-custom/css/custom.css",
                      "~/Content/intl-tel-input-master/build/css/intlTelInput.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/css/Login").Include(
                      "~/Content/font-awesome/font-awesome.css",
                      "~/Content/bootstrap.css",
                      "~/Content/admin-custom/css/custom.css",
                      "~/Content/nprogress/nprogress.css",
                      "~/Content/animate.css/animate.css"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/shared").Include(
                        "~/Scripts/fastclick/fastclick.js",
                        "~/Scripts/nprogress/nprogress.js",
                        "~/Scripts/Chart/Chart.min.js",
                        "~/Scripts/gauge/gauge.js",
                        "~/Scripts/bootstrap-progressbar/bootstrap-progressbar.min.js",
                        "~/Scripts/icheck/icheck.min.js",
                        "~/Scripts/skycons/skycons.js",
                        "~/Scripts/flot/jquery.flot.js",
                        "~/Scripts/flot/jquery.flot.pie.js",
                        "~/Scripts/flot/jquery.flot.time.js",
                        "~/Scripts/flot/jquery.flot.stack.js",
                        "~/Scripts/flot/jquery.flot.resize.js",
                        "~/Scripts/date/date.js",
                        "~/Scripts/custom/custom.min.js",
                        "~/Scripts/admin-custom/js/AdminCustom.js",
                        "~/Content/intl-tel-input-master/build/js/intlTelInput.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/dataTable").Include(
                "~/Scripts/DataTables/fnFindCellRowIndexes.js"
                ));
        }
    }
}
