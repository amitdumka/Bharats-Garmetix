

using Garmetix.Settings;
using Garmetix.Stores;
using Garmetix.Accounting;


namespace Garmetix.SRP
{
    // All the code in this file is included in all platforms.
    public static class GarmetixSRP
    {
        public static MauiAppBuilder UseGarmetixSRP(this MauiAppBuilder builder)
        {
            builder
                .UseStores() // Adds the Stores module to the application.
                .UseAccounting() // Adds the Accounting module to the application.
                .UseBanking() // Adds the Banking module to the application.
                .UseGarmetixSettings();// Adds the Settings service to the dependency injection container.



            return builder;
        }
    }
}
