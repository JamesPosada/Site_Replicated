using System.Web;
using System.Web.Optimization;

namespace ReplicatedSite
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/content/js/").Include());

            bundles.Add(new StyleBundle("~/content/css").Include());

            // Enable bundling optimizations, even when the site is in debug mode.
            BundleTable.EnableOptimizations = true;
        }
    }
}