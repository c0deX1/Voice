using System.Web;
using System.Web.Optimization;

namespace Voice
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // bundles.Add(new StyleBundle("~/Content/Grid/Styles"));
             bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                         "~/Scripts/jquery-3.1.1.min.js"));

             bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                         "~/Scripts/jquery.validate*"));
             bundles.Add(new ScriptBundle("~/bundles/p5").Include(
                  "~/Scripts/p5.min.js",
                  "~/Scripts/p5.sound.min.js"));
             bundles.Add(new ScriptBundle("~/bundles/triangle").Include(
                 "~/Scripts/triangle.js"));
             // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
             // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
             bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                         "~/Scripts/modernizr-*"));

             bundles.Add(new ScriptBundle("~/bundles/longer").Include(
                 "~/Scripts/longer.js",
                 "~/Scripts/piano.js"));

             bundles.Add(new ScriptBundle("~/bundles/vision").Include(
                 "~/Scripts/specialview.js"
                 ));

             bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Scripts/bootstrap.js",
                       "~/Scripts/respond.js"));

             bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.css",
                       "~/Content/site.css",
                       "~/Content/site.min.css",
                       "~/Content/common.css",
                       "~/Content/menu.css"));
             bundles.Add(new ScriptBundle("~/bundles/plotly").Include(
                 "~/Scripts/plotly_1.35.2.js"));
             bundles.Add(new ScriptBundle("~/bundles/HarmonicAnalysis").Include(
                "~/Scripts/HarmonicAnalysis.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
