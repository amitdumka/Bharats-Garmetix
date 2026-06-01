using Garmetix.Base.Views;

namespace Garmetix.Base
{
    public static class GarmetixBase
    {

        // List of Static Route name
        public const string RouteMainPage = "MainPage";
        public const string RouteSettingsPage = "SettingsPage";
        public const string DashboardPage = "Dashboard";
        public const string HomePage = "Dashboard";

        public const string LoginPage = "Login";
        public const string ErrorPage = "ErrorPage";

        public static MauiAppBuilder UseGarmetixBase(this MauiAppBuilder builder)
        {


            builder.Services.AddTransient<AboutUsPage>();
            builder.Services.AddTransient<ContactUsPage>();

            return builder;
        }

        public static async Task FailsafePage() { 
        
            await Shell.Current.GoToAsync(HomePage);
        }
        
        public static async Task GotoLoginPage() {
        
            await Shell.Current.GoToAsync( LoginPage);
        }
        public static async Task GotoHomePage() {

            await Shell.Current.GoToAsync(HomePage);

        }
        public static async Task GotoDashboardPage() {

            await Shell.Current.GoToAsync(DashboardPage);

        }


    }
}
