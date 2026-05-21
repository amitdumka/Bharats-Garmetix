using Bharat.ToolKits.Helpers;
using CommunityToolkit.Maui;
using Garmetix.Billing.PageModels;
using Garmetix.Billing.Pages;
using Garmetix.Billing.Services;

namespace Garmetix.Billing
{
    // All the code in this file is included in all platforms.
    public static class GarmetixBillingModule
    {
        /// <summary>
        /// Extension method to register Garmetix Billing services and handlers with the MauiAppBuilder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static MauiAppBuilder UseGarmetixBilling(this MauiAppBuilder builder)
        {
            // Register services, handlers, etc. here.
            // For example:
            // builder.Services.AddSingleton<IGarmetixBillingService, GarmetixBillingService>();

#if ANDROID
            builder.Services.AddSingleton<IPrintService, Garmetix.Billing.Platforms.Android.PrintService>();
#elif WINDOWS
            builder.Services.AddSingleton<IPrintService, Garmetix.Billing.Platforms.Windows.PrintService>();
#endif

            builder.Services.AddTransient<InvoiceEntryPage>();
            builder.Services.AddTransient<InvoiceHistoryPage>();
            builder.Services.AddTransient<InvoicesPageModel>();
            builder.Services.AddTransient<InvoiceEntryPageModel>();
            builder.Services.AddTransient<SaleInvoicePageModel>();
            builder.Services.AddSingleton<InvoiceService>();
            return builder;
        }


        /// <summary>
        /// Method to register any routes related to billing. This can be called during app initialization to ensure billing-related pages are accessible via routing.
        /// </summary>
        public static void RegisterBillingRoutes()
        {
            // Register any routes related to billing here.
            // For example:
            // Routing.RegisterRoute("billing", typeof(BillingPage));
            RouterHelper.AddRoute(typeof(InvoiceEntryPage));
            //RouterHelper.AddRoute(typeof(EditInvoicePage));
        }
    }
}
