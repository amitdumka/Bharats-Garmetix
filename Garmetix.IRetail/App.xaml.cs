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
using Garmetix.Authentication.Pages;
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

        public App(IDatabaseService ds)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Lic);
            CultureInfo.CurrentUICulture = new CultureInfo("en-IN");
            InitializeComponent();
            // Register services and view models here if using dependency injection
            BharatGarmetixModules.RegisterPaeRoutes();
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