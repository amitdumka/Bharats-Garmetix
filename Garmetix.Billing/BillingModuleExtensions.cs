using Garmetix.AI.Billing.Services;
using Garmetix.AI.Billing.ViewModels;
using Garmetix.AI.Billing.Views;
using Garmetix.Billing.Platforms.Android;

namespace Garmetix.Billing
{
    // All the code in this file is included in all platforms.
    public static class BillingModuleExtensions
    {
        //TODO: This is for Billing and Inventory 

        /// <summary>
        /// Enabling AI Billing services and platform-specific printing services for the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static MauiAppBuilder EnableAIBilling(this MauiAppBuilder builder)
        {
            // Register AI Billing services and dependencies here
            // e.g., builder.Services.AddSingleton<IAIBillingService, AIBillingService>();


            // Register Platform-Specific Printing
#if ANDROID
            builder.Services.AddSingleton<IPrintService, PrintService>();
#elif WINDOWS
            builder.Services.AddSingleton<IPrintService,  Garmetix.AI.Billing.Platforms.Windows.PrintService>();
#endif
            builder.Services.AddTransient<InvoiceEntryViewModel>();
            builder.Services.AddTransient<InvoiceEntryPage>();
            return builder;
        }
    }
}
