using System.Web;
using System.Web.Optimization;

namespace MyShop
{
    public class BundleConfig
    {
        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/jquery.nice-select.min.js",
                        "~/Scripts/jquery-ui.min.js",
                        "~/Scripts/jquery.slicknav.js",
                        "~/Scripts/mixitup.min.js",
                        "~/Scripts/owl.carousel.min.js",
                        "~/Scripts/main.js",
                        "~/Scripts/es6-promise.js",
                        "~/Scripts/es6-promise.auto.js",
                        "~/Scripts/vue.js",
                        "~/Scripts/axios.min.js",
                        "~/Scripts/app.js"));
            

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/elegant-icons.css",
                      "~/Content/nice-select.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/owl.carousel.min.css",
                      "~/Content/slicknav.min.css",
                      "~/Content/style.css"));
        }
    }
}
