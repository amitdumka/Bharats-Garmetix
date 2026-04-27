using Garmetix.Authentication.Pages;
using Garmetix.Onboarding.Pages;
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
            GarmetixModulues.RegisterPageRoutes();
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
