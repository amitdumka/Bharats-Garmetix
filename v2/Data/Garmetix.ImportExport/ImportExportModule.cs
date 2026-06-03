namespace Garmetix.ImportExport
{
    // All the code in this file is included in all platforms.
    public static class ImportExportModule
    {
        public  static MauiAppBuilder UseImportExport(this MauiAppBuilder builder)
        {
            // Register services here
            return builder;
        }

        public static void UseRoute()
        {
           // Routing.RegisterRoute("importexport", typeof(ImportExportPage));
        }
    }
}
