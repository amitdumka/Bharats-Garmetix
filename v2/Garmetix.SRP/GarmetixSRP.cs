

using Garmetix.Accounting;
using Garmetix.Authentication;
using Garmetix.Authentication.Pages;
using Garmetix.Commons;
using Garmetix.Core.PdfServices;
using Garmetix.DataServices;
using Garmetix.HRM;
using Garmetix.Onboarding;
using Garmetix.Onboarding.Pages;
using Garmetix.Settings;
using Garmetix.Stores;
using System.Globalization;


namespace Garmetix.SRP
{
    // All the code in this file is included in all platforms.
    public static class GarmetixSRP
    {
        /// <summary>
      /// Key of Synfusion
      /// </summary>
        public const string SyncKey = "Ngo9BigBOggjHTQxAR8/V1JHaF1cXmhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdENjXX1YcXBURmVbV0x+XEleYA==";
        public static async Task InitApp()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(GarmetixSRP.SyncKey);
            CultureInfo.CurrentUICulture = new CultureInfo("en-IN");
            GarmetixSRP.RegisterGarmetixSRPRoutes();
        }
        public static MauiAppBuilder UseGarmetixSRP(this MauiAppBuilder builder)
        {
            builder
                .UseCoreModule() // Adds the Core module to the application.
                .UseOnboarding() // Adds the Onboarding module to the application.
                .UseAuthentication() // Adds the Authentication module to the application.
                .UseStores() // Adds the Stores module to the application.
                .UseAccounting() // Adds the Accounting module to the application.
                .UseBanking() // Adds the Banking module to the application.
                .UseHRM() // Adds the HRM module to the application.
                .UseGarmetixSettings();// Adds the Settings service to the dependency injection container.



            return builder;
        }
        public static MauiAppBuilder UseaGarmetixSRPService(this MauiAppBuilder builder)
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

        public static async Task<Window> CreateMainWindow(IActivationState? activationState, Shell appShell)
        {

            // Check if user has previously logged in and set a PIN
            string? hasPin = await SecureStorage.Default.GetAsync("HasPin");

            if (hasPin == "true")
            {
                // Bypass full login, go straight to Quick PIN Unlock
                //await Shell.Current.GoToAsync(PinUnlockPageUrl(appShell));
                return new Window(new PinUnlockPage(appShell));
            }

            // Check if onboarding is complete
            var onboardingComplete = Preferences.Get("IsOnboardingComplete", false);

            if (!onboardingComplete)
            {
                // Use a NavigationPage to allow navigation between onboarding/seed pages
                return new Window(new NavigationPage(new OnboardingChoicePage(new Login(appShell))));
            }
            else
            {
                // if android and ios then 

                // var session = SessionHelper.TryLoadStoreSession().Result;

                //if (!(session == null || !session.IsAutoLoginEnabled))
                //{
                //    return new Window(new AppShell());
                // }
                // else
                {
                    return new Window(new Login(appShell));
                }
            }

        }


    }
}
