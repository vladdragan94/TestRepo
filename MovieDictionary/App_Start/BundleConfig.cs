using System.Web;
using System.Web.Optimization;

namespace MovieDictionary
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

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/Details").Include(
                        "~/Scripts/Modules/Details.js"));

            bundles.Add(new ScriptBundle("~/bundles/Main").Include(
                       "~/Scripts/handlebars.js",
                       "~/Scripts/Modules/Alerter.js",
                       "~/Scripts/Modules/AjaxController.js",
                       "~/Scripts/Modules/Facebook.js",
                       "~/Scripts/Modules/Main.js"));

            bundles.Add(new ScriptBundle("~/bundles/MovieDetails").Include(
                      "~/Scripts/Modules/MovieDetails.js"));

            bundles.Add(new ScriptBundle("~/bundles/MoviesSearch").Include(
                      "~/Scripts/Modules/MoviesSearch.js"));

            bundles.Add(new ScriptBundle("~/bundles/Forum").Include(
                      "~/Scripts/Modules/Forum.js"));

            bundles.Add(new ScriptBundle("~/bundles/PostDetails").Include(
                      "~/Scripts/Modules/PostDetails.js"));

            bundles.Add(new ScriptBundle("~/bundles/Users").Include(
                      "~/Scripts/Modules/Users.js"));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/MovieDetails").Include(
                      "~/Content/MovieDetails.css"));

            bundles.Add(new StyleBundle("~/Content/Forum").Include(
                      "~/Content/Forum.css"));

            bundles.Add(new StyleBundle("~/Content/MoviesSearch").Include(
                      "~/Content/MoviesSearch.css"));

            bundles.Add(new StyleBundle("~/Content/Users").Include(
                      "~/Content/Users.css"));
        }
    }
}
