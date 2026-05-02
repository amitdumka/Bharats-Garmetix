using Garmetix.Commons.Dashboard.PageModels;
using Garmetix.CoreBase.Dashboard.Pages;
using Garmetix.CoreServices.Dashboard;

namespace Garmetix.Commons.Dashboard
{
    public static class DashboardModule
    {
        public static MauiAppBuilder EnableDashboard(this  MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<DashboardService>();//TODO: for time being it is here it should be move to shared module
            builder.Services.AddSingleton<DefaultDashboardPageModel>();
            builder.Services.AddSingleton<DashboardPage>();

            return builder;
        }
    }
}
