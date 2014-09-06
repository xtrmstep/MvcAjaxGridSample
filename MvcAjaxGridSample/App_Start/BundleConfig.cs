using System.Web.Optimization;

namespace MvcAjaxGridSample
{
    public class BundleConfig
    {
        public static void RegisterBundle(BundleCollection bundle)
        {
            // scripts
            bundle.Add(new ScriptBundle("~/scripts/core").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/bootstrap.js"
                ));

            //styles
            bundle.Add(new StyleBundle("~/styles/core").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css"
                ));

#if DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}