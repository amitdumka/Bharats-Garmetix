using Garmetix.ImportExport.Import;
using Garmetix.ImportExports.Services;
using Garmetix.ImportExports.ViewModels;

namespace Garmetix.ImportExport
{
    // All the code in this file is included in all platforms.
    public static class ImportExportModule
    {
        public  static MauiAppBuilder UseImportExport(this MauiAppBuilder builder)
        {
            // Register services here

            builder.Services.AddTransient<PurchaseDatabaseSyncService>();
            builder.Services.AddTransient<PurchaseImportService>();
            builder.Services.AddTransient<PurchaseImportViewModel>();
            builder.Services.AddTransient<PurchaseImportPage>();
            builder.Services.AddTransient<CategoryMappingService>();
          
            

            return builder;
        }

        public static void UseRoute()
        {
            // Routing.RegisterRoute("importexport", typeof(ImportExportPage));
            Routing.RegisterRoute("PurchaseImport",typeof(PurchaseImportPage));
        }
    }
}
