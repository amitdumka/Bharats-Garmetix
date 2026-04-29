using Garmetix.Accounting;
using Garmetix.Core.Services;
using Garmetix.CoreBase.Dashboard;
using Garmetix.CoreBase.DayOperations.Pages;
using Garmetix.CoreBase.HRM;
using Garmetix.CoreBase.TimeLines;
using Garmetix.Stores;

namespace Garmetix.Commons
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

            builder.UseAccounting().UseBanking().EnableDashboard().UseHRM().UseStores();

          

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
