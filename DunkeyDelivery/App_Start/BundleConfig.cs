using System.Web;
using System.Web.Optimization;

namespace DunkeyDelivery
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
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/fastclick").Include(
                  "~/Scripts/fastclick/fastclick.js"));
            bundles.Add(new ScriptBundle("~/bundles/nprogress").Include(
                  "~/Scripts/nprogress/nprogress.js"));
            bundles.Add(new ScriptBundle("~/bundles/Chart").Include(
                  "~/Scripts/Chart/Chart.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/gauge").Include(
                 "~/Scripts/gauge/gauge.min.js")); 
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-progressbar").Include(
                 "~/Scripts/bootstrap-progressbar/bootstrap-progressbar.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/icheck").Include(
                "~/Scripts/icheck/icheck.min.js")); 

            bundles.Add(new ScriptBundle("~/bundles/skycons").Include(
                "~/Scripts/skycons/skycons.js")); 
            bundles.Add(new ScriptBundle("~/bundles/flot").Include(
                "~/Scripts/flot/jquery.flot.js",
                "~/Scripts/flot/jquery.flot.pie.js",
                "~/Scripts/flot/jquery.flot.time.js",
                "~/Scripts/flot/jquery.flot.stack.js",
                "~/Scripts/flot/jquery.flot.resize.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/date").Include(
                "~/Scripts/date/date.js")); 

            bundles.Add(new ScriptBundle("~/bundles/daterangepicker").Include(
                "~/Scripts/daterangepicker/daterangepicker.js")); 

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                "~/Scripts/custom/custom.min.js"));





            bundles.Add(new Bundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/Admin-Custom").Include(
                    "~/Content/admin-custom/css/custom.css"));

            bundles.Add(new StyleBundle("~/Content/animate").Include(
                    "~/Content/animate.css/animate.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                   "~/Content/font-awesome/css/font-awesome.css",
                   "~/Content/font-awesome/css/font-awesome.css.map"
                   ));



        }
    }
}
