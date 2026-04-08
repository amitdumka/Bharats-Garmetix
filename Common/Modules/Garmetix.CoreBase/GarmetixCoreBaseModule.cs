using Garmetix.Core.Services;
using Garmetix.CoreBase.Accounting;
using Garmetix.CoreBase.Dashboard;
using Garmetix.CoreBase.DayOperations.Pages;
using Garmetix.CoreBase.HRM;
using Garmetix.CoreBase.Stores;
using Garmetix.CoreBase.TimeLines;

namespace Garmetix.CoreBase
{
    // All the code in this file is included in all platforms.
    public static class GarmetixCoreBaseModule
    {
        public static MauiAppBuilder UseCoreModule(this MauiAppBuilder builder)
        {
            //TimeLine
            builder.Services.AddSingleton<TimelineDataService>();
            builder.Services.AddSingleton<TimelinePageModel>();
            builder.Services.AddTransient<TimelinePage>();

            //DayOperation
            builder.Services.AddTransient<DayBeginEntyPage>();
            builder.Services.AddTransient<DayEndEntryPage>();
            builder.Services.AddTransient<PettyCashSheetEntryPage>();

            builder.EnableAccounting().EnableBanking().EnableDashboard().EnableHRM().EnableStore();

          

            return builder;
        }

        public static void EnableCoreModulesRoutes()
        {
            //Register RouterH

            RouterHelper.AddRoute(typeof(DayBeginEntyPage));
            RouterHelper.AddRoute(typeof(DayEndEntryPage));
            RouterHelper.AddRoute(typeof(PettyCashSheetEntryPage));

            AccountingModule.EnableRoutes();
            HRMModules.EnableRoutes();
            ClientModule.EnableRoutes();
            
            
        }

    }
}
