using System.Web;
using System.Web.Optimization;

namespace MauloaDemo.Customer {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/underscore").Include(
                        "~/Scripts/underscore.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/kendo/kendocss").Include(
                        "~/Content/kendo/kendo.common.min.css",
                        "~/Content/kendo/kendo.common.core.min.css",
                        "~/Content/kendo/kendo.uniform.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/kendo/kendoui").Include(
                        "~/Scripts/kendo/kendo.all.min.js",
                        "~/Scripts/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new StyleBundle("~/Content/alertifycss").Include(
                        "~/Content/alertify/alertify.core.css",
                        "~/Content/alertify/alertify.bootstrap.css",
                        "~/Content/alertify/alertify.custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/alertify").Include(
                        "~/Scripts/alertify/alertify.js"));

            bundles.Add(new StyleBundle("~/Content/toastrcss").Include("~/Content/toastr.css"));
            bundles.Add(new ScriptBundle("~/bundles/toastr").Include("~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/Scripts/moment.js"));

            bundles.Add(new StyleBundle("~/Content/structure").Include(
                        "~/Content/normalize.css",
                        "~/Content/structure.css"));

            bundles.Add(new StyleBundle("~/Content/stylesheet").Include(
                        "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/stylesheet").Include(
                        "~/common/css/common.css",
                        "~/common/css/normalize.css"));

        }
    }
}
