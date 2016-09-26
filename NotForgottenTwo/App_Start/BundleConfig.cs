using System.Web;
using System.Web.Optimization;

namespace NotForgottenTwo
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
                      "~/Scripts/respond.js,",
                      "~/Scripts/jasny-bootstrap.min.js",
                            //  "~/Scripts/bootstrap-typeahead.js",
                            // "~/Scripts/jquery.typeahead.min.js",
                            "~/Scripts//typeahead.mvc.model.js",
            "~/Scripts/SiteScripts.js"));
      


        bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/custom.css",
                      "~/Content/jasny-bootstrap.min.css"));


            //add link to jquery on the CDN
            //var jasnybootstrapCdnPath = "//cdnjs.cloudflare.com/ajax/libs/jasny-bootstrap/3.1.3/js/jasny-bootstrap.min.js";

            //bundles.Add(new ScriptBundle("~/bundles/jansy",
            //            jasnybootstrapCdnPath).Include("~/Scripts/jasny-bootstrap.min.js"));


            //var  jasnybootCCstrapCdnPath = "//cdnjs.cloudflare.com/ajax/libs/jasny-bootstrap/3.1.3/css/jasny-bootstrap.min.css";



            //bundles.Add(new StyleBundle("~/Content/jansy", jasnybootCCstrapCdnPath).Include(
            //          "~/Content/jasny-bootstrap.min.css"));


        }
    }
}
