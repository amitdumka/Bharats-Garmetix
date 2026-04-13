using Garmetix.AI.Billing.ViewModels;
using Garmetix.AI.Billing.Views;
using Garmetix.Billing.AIBased.Services; 
#if ANDROID
using Garmetix.AI.Billing.Platforms.Android;
#endif
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
            builder.Services.AddTransient<InvoiceHistoryViewModel>();
            builder.Services.AddTransient<Garmetix.AI.Billing.Views.InvoiceHistoryPage>();
            builder.Services.AddTransient<Garmetix.AI.Billing.ViewModels.EditInvoiceViewModel>();
            builder.Services.AddTransient<Garmetix.AI.Billing.Views.EditInvoicePage>();
           
            // Sales Routes
            Routing.RegisterRoute("InvoiceEntryPage", typeof(InvoiceEntryPage));
            Routing.RegisterRoute("EditInvoicePage", typeof(EditInvoicePage));

            // NEW: Purchase Route
            Routing.RegisterRoute("PurchaseEntryPage", typeof(PurchaseEntryPage));
            // --- PURCHASE MODULE ---
            builder.Services.AddTransient<Garmetix.AI.Billing.ViewModels.PurchaseHistoryViewModel>();
            builder.Services.AddTransient<Garmetix.AI.Billing.Views.PurchaseHistoryPage>();

            builder.Services.AddTransient<Garmetix.AI.Billing.ViewModels.PurchaseEntryViewModel>();
            builder.Services.AddTransient<Garmetix.AI.Billing.Views.PurchaseEntryPage>();
           
            builder.Services.AddTransient<Garmetix.AI.Billing.ViewModels.PaymentHistoryViewModel>();
            builder.Services.AddTransient<Garmetix.AI.Billing.Views.PaymentRegistryPage>();

            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<DashboardPage>();
            return builder;
        }



        public static MauiAppBuilder EnableBillingAndInventory(this MauiAppBuilder builder)
        {
            // Register AI Inventory services and dependencies here
            // e.g., builder.Services.AddSingleton<IAIInventoryService, AIInventoryService>();




            return builder;
        }
    }
}