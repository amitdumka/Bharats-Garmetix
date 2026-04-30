

using Garmetix.Settings;
using Garmetix.Stores;
using Garmetix.Accounting;
using Garmetix.HRM;
using Garmetix.Authentication;
using Garmetix.Onboarding;
using Garmetix.Core.PdfServices;
using Garmetix.CoreBase;
using Garmetix.DataServices;
using Garmetix.Core;
using Garmetix.Commons;


namespace Garmetix.SRP
{
    // All the code in this file is included in all platforms.
    public static class GarmetixSRP
    {
        public static MauiAppBuilder UseGarmetixSRP(this MauiAppBuilder builder)
        {
            builder
                .UseOnboarding() // Adds the Onboarding module to the application.
                .UseAuthentication() // Adds the Authentication module to the application.
                .UseStores() // Adds the Stores module to the application.
                .UseAccounting() // Adds the Accounting module to the application.
                .UseBanking() // Adds the Banking module to the application.
                .UseHRM() // Adds the HRM module to the application.
                .UseGarmetixSettings();// Adds the Settings service to the dependency injection container.



            return builder;
        }
        public static MauiAppBuilder UseaGarmetixService(this MauiAppBuilder builder)
        {
            builder
                .UseDataServices()   // Enable the Data Services module
                .UseCoreModule()      // Enable the Core module
                .UsePdfServices()  // Enable the PDF Services module
                .UseDataServices();   // Enable the Data Services module
          
            return builder;
        }
        public static void RegisterGarmetixSRPRoutes()
        {
            AuthenticationModule.RegisterAuthenticationRoute(); // Registers the routes for the Authentication module.
            HRMModules.EnableHRMRoutes(); // Registers the routes for the HRM module.
            AccountingModule.EnableAccountingRoutes(); // Registers the routes for the Accounting module.
            ClientModule.RegisterStoreRoutes(); // Registers the routes for the Stores module.
            GarmetixSettingsModule.RegisterSettingRoute(); // Registers the routes for the Settings module.
        }
    }
}
