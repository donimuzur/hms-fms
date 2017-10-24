using System.Web;
using System.Web.Optimization;

namespace FMS.Website
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/css/app.css",
                      "~/Content/css/bootstrap.min.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            
            bundles.Add(new StyleBundle("~/bundles/fmscss").Include(
                "~/Content/App/bootstrap/bootstrap.css",
                "~/Content/App/print.css",
                "~/Content/App/bootstrap/bootstrap-datepicker3.min.css",
                "~/Content/App/jquery/jquery-ui.min.css",
                "~/Content/App/jquery/jquery-ui.theme.min.css",
                "~/Content/App/jquery/jquery-growl.min.css",
                "~/Content/App/app.css",
                "~/Content/App/open-sans.font.min.css",
                "~/Content/App/font-awesome/css/font-awesome.min.css",
                "~/Content/App/tps-eproc.css",
                "~/Content/App/custom_front.css",
                "~/Content/App/treeGrid.css",
                "~/Content/App/skins/_all-skins.min.css",
                "~/Content/App/angular-growl.css",
                "~/Content/App/main3.css",
                "~/Content/App/paneltab.css",
                //"~/Content/App/angular-ui-select/select.min.css",
                "~/Content/App/sweetalert.css",
                "~/Content/App/AdminLTE.css"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryfms").Include(
                "~/Scripts/Lib/lib/jquery/jquery.js",
                "~/Scripts/Lib/lib/jquery/jquery-print.js",
                "~/Scripts/Lib/lib/jquery/jquery-md5.min.js",
                "~/Scripts/Lib/lib/jquery/jquery-blockUI.js",
                "~/Scripts/Lib/lib/jquery/jquery-growl.js",
                "~/Scripts/Lib/lib/jquery/jquery-masked-input.js",
                "~/Scripts/Lib/lib/jquery/jquery-ui.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                      "~/Scripts/js/main.min.js",
                      "~/Scripts/js/popper.js",
                      "~/Scripts/js/bootstrap.min.js"
                ));
            
            bundles.Add(new ScriptBundle("~/bundles/libs").Include(
                "~/Scripts/Lib/lib/momentjs/moment.js",
                "~/Scripts/Lib/lib/bootstrap/bootstrap.js",
                "~/Scripts/Lib/lib/bootstrap/bootstrap-datepicker.js",
                "~/Scripts/Lib/lib/bootstrap/bootstrap-datetimepicker.min.js",
                //"~/Scripts/Lib/lib/angular/ui/bootstrap/ui-bootstrap-datetimepicker.js",
                //"~/Scripts/Lib/lib/angular/ui/bootstrap/ui-bootstrap.js",
                //"~/Scripts/Lib/lib/angular/ui/bootstrap/ui-bootstrap-tpls.js",
                //"~/Scripts/Lib/lib/angular/ui/ui-mask.min.js",
                "~/Scripts/Lib/lib/bootboxjs/bootbox.js",
                "~/Scripts/Lib/lib/datatables/jquery.datatables.js",
                //"~/Scripts/Lib/lib/angular/datatables/angular-datatables.min.js",
                "~/Scripts/Lib/lib/datatables/bootstrap.datatables.js",
                "~/Scripts/Lib/lib/angular/datatables/plugins/bootstrap/angular-datatables.bootstrap.min.js",
                "~/Scripts/Lib/lib/oclazyload/ocLazyLoad.js",
                "~/Scripts/Lib/lib/alasql/alasql.min.js",
                "~/Scripts/Lib/lib/alasql/xlsx.core.min.js",
                //"~/Scripts/Lib/bower_components/angular-ui-select/dist/select.js",
                "~/Scripts/Lib/lib/socket.io/socket.io.js",
                "~/Scripts/Lib/tree-grid-directive.js",
                "~/Scripts/Lib/lib/adminlte.js",
                "~/Scripts/Lib/lib/app.js",
                //"~/Scripts/Lib/lib/angular-adminlte.min.js",
                "~/Scripts/Lib/lib/slimScroll/jquery.slimscroll.min.js",
                "~/Scripts/Lib/lib/sweetalert/dist/sweetalert.min.js",
                //"~/Scripts/Lib/lib/ng-sweet-alert/ng-sweet-alert.js",
                "~/Scripts/Lib/lib/tinymce/tinymce.js",
                //"~/Scripts/Lib/lib/angular/angular-currency-mask.js",
                "~/Scripts/Lib/wow.js",
                "~/Scripts/Lib/main.js",
                "~/Scripts/Lib/Backstretch.js",
                "~/Scripts/Lib/custom.js",
                "~/Scripts/Lib/plugin/amcharts/amcharts/amcharts.js",
                "~/Scripts/Lib/plugin/amcharts/amcharts/serial.js",
                "~/Scripts/Lib/plugin/amcharts/amcharts/pie.js",
                "~/Scripts/Lib/plugin/amcharts/amcharts/themes/light.js"
                ));

        }
    }
}