using Garmetix.Base.Views;

namespace Garmetix.Base
{
    public static class GarmetixBase
    {

        // List of Static Route name
        public const string RouteMainPage = "MainPage";
        public const string RouteSettingsPage = "SettingsPage";
        public const string DashboardPage = "DashboardPage";

        public const string RouteLoginPage = "LoginPage";
        public const string ErrorPage = "ErrorPage";

        public static MauiAppBuilder UseGarmetixBase(this MauiAppBuilder builder)
        {


            builder.Services.AddTransient<AboutUsPage>();
            builder.Services.AddTransient<ContactUsPage>();

            return builder;
        }
    }
}
