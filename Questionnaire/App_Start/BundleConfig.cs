using System.Web.Optimization;

namespace QuestionnaireProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/jquery-confirm.min.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/moment-jalaali.js",
                      "~/components/persian-datepicker/persian-date.min.js",
                      "~/components/persian-datepicker/persian-datepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminlte").Include(
                //"~/assets/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                //"~/assets/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/components/fastclick/lib/fastclick.js",
                "~/assets/dist/js/adminlte.min.js",
                "~/assets/dist/js/pages/dashboard.js",
                "~/assets/dist/js/demo.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/adminlteExtra").Include(
                //"~/assets/bower_components/raphael/raphael.min.js",
                //"~/assets/bower_components/morris.js/morris.min.js",
                //"~/assets/bower_components/jquery-sparkline/dist/jquery.sparkline.min.js",
                //"~/assets/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                //"~/assets/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                //"~/assets/bower_components/jquery-knob/dist/jquery.knob.min.js",
                //"~/Scripts/moment.js",
                //"~/assets/bower_components/bootstrap-daterangepicker/daterangepicker.js",
                //"~/assets/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                //"~/assets/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                //"~/assets/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/components/fastclick/lib/fastclick.js",
                "~/assets/dist/js/adminlte.min.js",
                "~/assets/dist/js/pages/dashboard.js",
                "~/assets/dist/js/demo.js"              
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css",
                "~/css/jquery-confirm.min.css",
                "~/components/persian-datepicker/persian-datepicker.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/myCss").Include(               
                "~/assets/dist/css/bootstrap-theme.css",
                "~/assets/dist/css/rtl.css",
                "~/components/font-awesome/css/font-awesome.min.css",
                "~/components/Ionicons/css/ionicons.min.css",
                "~/assets/dist/css/AdminLTE.css",
                "~/assets/dist/css/skins/_all-skins.min.css",
                "~/assets/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css"
            ));

            bundles.Add(new StyleBundle("~/Content/extraCss").Include(
                //"~/assets/dist/css/persian-datepicker-0.4.5.min.css",
                //"~/assets/bower_components/morris.js/morris.css",
                //"~/assets/bower_components/jvectormap/jquery-jvectormap.css",
                //"~/assets/bower_components/bootstrap-daterangepicker/daterangepicker.css"
            ));




        }
    }
}
