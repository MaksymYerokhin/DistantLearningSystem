using System.Web.Optimization;

namespace DistantLearningSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            #region Dashboard

            bundles.Add(new ScriptBundle("~/bundles/ControlPanelScripts").Include(
                "~/Scripts/Dashboard/jquery-{version}.js",
                "~/Scripts/Dashboard/bootstrap.min.js",
                "~/Scripts/Dashboard/jquery.metisMenu.js",
                "~/Scripts/Dashboard/sb-admin.js"
                ));

            bundles.Add(new StyleBundle("~/bundles/ControlPanelStyles").Include(
                "~/Content/Dashboard/bootstrap.min.css",
                "~/Content/Dashboard/font-awesome.css",
                "~/Content/Dashboard/iconmoon.css",
                "~/Content/Dashboard/sb-admin.css"
            ));

            #endregion

            #region DataTableBundles

            bundles.Add(new ScriptBundle("~/bundles/DataTablesScripts").Include(
                "~/Plugins/DataTables/jquery.dataTables.js",
                "~/Plugins/DataTables/dataTables.bootstrap.js"));

            bundles.Add(new StyleBundle("~/bundles/DataTablesStyles").Include(
                "~/Plugins/DataTables/*.css"));

            #endregion
          
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/Custom/custom.js"));
            
            #region JqueryUI

            bundles.Add(new StyleBundle("~/bundles/JqueryUI/Styles").Include("~/Content/jquery-ui.css"));
            bundles.Add(new ScriptBundle("~/bundles/JqueryUI/Scripts").Include("~/Scripts/jquery-ui.js"));

            #endregion
        }
    }
}