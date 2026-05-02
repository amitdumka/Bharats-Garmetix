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
        }
    }
}
