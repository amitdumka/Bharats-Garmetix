//namespace Garmetix.IRetail
//{
//    public partial class App : Application
//    {
//        public App()
//        {
//            InitializeComponent();
//        }

//        protected override Window CreateWindow(IActivationState? activationState)
//        {
//            return new Window(new AppShell());
//        }
//    }
//}
using Bharat.ToolKits.Helpers;
using Garmetix.AI.Billing.Views;
using Garmetix.Authentication.Pages;
using Garmetix.CoreBase.DayOperations.Pages;
using Garmetix.Databases.Services;
using Garmetix.IRetail;
using Garmetix.Onboarding.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Garmetix.IRetail
{
    public partial class App : Application
    {
        // private readonly string Lic29x = "Ngo9BigBOggjHTQxAR8/V1NNaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXpfcXRSQ2hZV0xwWkNWYUA=";
        private readonly string Lic = "Mzk1MTMzN0AzMzMwMmUzMDJlMzAzYjMzMzAzYlNlK0JXeWU0YmtIZy80aWhmMkNBRXZoZ3lodHdFSE9vU1JobWJGZGRSMWs9";
        private readonly string tempLIc = "Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCekx3Q3xbf1x2ZFREallUTndbUj0eQnxTdENjXX9XcXZQQ2FYVEBwWEleYA==";
        public App(IDatabaseService ds)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(tempLIc);
            CultureInfo.CurrentUICulture = new CultureInfo("en-IN");
            InitializeComponent();
            // Register services and view models here if using dependency injection
            BharatGarmetixModules.RegisterPaeRoutes();
            Routing.RegisterRoute("//LoginPage", typeof(LoginPage));
        }
        protected override async void OnStart()
        {
            base.OnStart();

            // Check if user has previously logged in and set a PIN
            string hasPin = await SecureStorage.Default.GetAsync("HasPin");

            if (hasPin == "true")
            {
                // Bypass full login, go straight to Quick PIN Unlock
                await Shell.Current.GoToAsync("//PinUnlockPage");
            }
            else
            {
                // First time running, or user logged out
                if (Application.Current?.MainPage is Shell)
                    await Shell.Current.GoToAsync("//LoginPage");
                else
                    Application.Current.MainPage = new AppShell(); // then navigate
            }
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            //return new Window(new AppShell());
            var onboardingComplete = Preferences.Get("IsOnboardingComplete", false);
            if (!onboardingComplete)
            {
                // Use a NavigationPage to allow navigation between onboarding/seed pages
                return new Window(new NavigationPage(new OnboardingChoicePage(new Login(new AppShell()))));
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
                    return new Window(new Login(new AppShell()));
                }

            }
        }
    }
}