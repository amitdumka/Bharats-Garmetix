using Garmetix.Authentication;
using Garmetix.Authentication.Pages;
using Garmetix.CoreBase;
using Garmetix.CoreBase.Accounting;
using Garmetix.CoreBase.Dashboard;
using Garmetix.CoreBase.HRM;
using Garmetix.CoreBase.Stores;
using Garmetix.CoreServices;
using Garmetix.DataServices;
using Garmetix.Onboarding;
using Garmetix.Onboarding.Pages;
using Garmetix.PDFServices;
using Garmetix.Reports;
using System.Globalization;

namespace Garmetix
{
    /// <summary>
    /// 
    /// </summary>
    public static class GarmetixHelpers
    {
        public static async Task InitApp()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(GarmetixModulues.SyncKey);
            CultureInfo.CurrentUICulture = new CultureInfo("en-IN");
            GarmetixModulues.RegisterPaeRoutes();
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
    public static class GarmetixModulues
    {

        public const string SyncKey = "Ngo9BigBOggjHTQxAR8/V1JHaF1cXmhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdENjXX1YcXBURmVbV0x+XEleYA==";
        public static void RegisterPaeRoutes()
        {
            // Register any additional routes here if needed
            GarmetixCoreBaseModule.EnableCoreModulesRoutes();
            OnboardingModule.RegisterRouteOnboarding();
            AuthenticationModule.RegisterAuthenticationRoute();
        }

        public static MauiAppBuilder ConfigureGarmetixModules(this MauiAppBuilder builder)
        {

            // Enabling Syncfusion Libraries
            builder


             // Registering Garmetix Modules
             .RegiserService().
             // Registering Garmetix Modules
             RegisterModules();
            // Enabling Fonts

            return builder;
        }

        public static MauiAppBuilder RegisterModules(this MauiAppBuilder builder)
        {
            builder.EnableAccounting().EnableBanking().EnableHRM().UseAuthentication();
            builder.EnableDashboard().EnableOnBoarding().EnableStore().UseReporting();


            //Remove this line if you don't want to enable AI billing in your app, or if you want to enable it separately in specific modules.
            //builder.EnableAIBilling().UseGarmetixSettings();
            return builder;
        }
        public static MauiAppBuilder UseGarmetixDefaultModules(this MauiAppBuilder builder)
        {
            builder
                //.UseGarmetixCoreServices()
                .EnablePdfServices().UseCoreModule().UseDataServices();

            return builder;
        }

        public static MauiAppBuilder RegiserService(this MauiAppBuilder builder)
        {
            builder.UseGarmetixCoreServices();//.UseGarmetixDatabases();
            builder.EnablePdfServices().UseCoreModule().UseDataServices();

            return builder;
        }




    }
}
