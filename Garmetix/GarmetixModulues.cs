using Garmetix.Accounting;
using Garmetix.Authentication;
using Garmetix.Commons;
using Garmetix.Core.PdfServices;
using Garmetix.CoreBase.Dashboard;
using Garmetix.DataServices;
using Garmetix.Onboarding;
using Garmetix.Stores;
using Garmetix.HRM;

namespace Garmetix
{
    public static class GarmetixModulues
    {
        /// <summary>
        /// Key of Synfusion
        /// </summary>
        public const string SyncKey = "Ngo9BigBOggjHTQxAR8/V1JHaF1cXmhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdENjXX1YcXBURmVbV0x+XEleYA==";
        
        /// <summary>
        /// Register the Route of All modules
        /// </summary>
        public static void RegisterPageRoutes()
        {
            // Register any additional routes here if needed
            //Core Base Library
            GarmetixCoreBaseModule.EnableCoreModulesRoutes();
            // Onboarding Module
            OnboardingModule.RegisterRouteOnboarding();
            // Authentication Module
            AuthenticationModule.RegisterAuthenticationRoute();
        }

        public static MauiAppBuilder ConfigureGarmetix(this MauiAppBuilder builder)
        {
            builder
             // Registering Garmetix Modules
             .UseGarmetixService()
             // Registering Garmetix Modules
             .UseGarmetixModules();
             return builder;
        }

        public static MauiAppBuilder UseGarmetixModules(this MauiAppBuilder builder)
        {
            builder
                .UseAccounting() // Enable the Accounting module
                .UseBanking() // Enable the Banking module
                .UseHRM() // Enable the HRM module
                .UseAuthentication(); // Enable the Authentication module
            builder
                .EnableDashboard()  // Enable the Dashboard module
                .EnableOnBoarding() // Enable the Onboarding module
                .UseStores();   // Enable the Store module
                //.UseReporting();    // Enable the Reporting module
            return builder;
        }
        public static MauiAppBuilder UseGarmetixService(this MauiAppBuilder builder)
        {
            builder
                .UsePdfServices()  // Enable the PDF Services module
                .UseCoreModule()      // Enable the Core module
                .UseDataServices();   // Enable the Data Services module

            return builder;
        }





    }
}
