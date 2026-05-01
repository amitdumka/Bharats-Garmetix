using Garmetix.Accounting;
using Garmetix.Commons.Dashboard.PageModels;
using Garmetix.Commons.DayOperations.PageModels;
using Garmetix.CoreBase.Dashboard;
using Garmetix.CoreBase.Dashboard.Pages;
using Garmetix.CoreBase.DayOperations.Pages;
using Garmetix.CoreBase.TimeLines;
using Garmetix.CoreServices.Dashboard;
using Garmetix.Services;
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



            builder.Services.AddTransient<PettyCashSheetFormPageModel>();
            builder.Services.AddTransient<CashDetailFormPageModel>();

            builder.Services.AddTransient<PettyCashSheetPageModel>();
            builder.Services.AddTransient<PettyCashSheetPage>();
            builder.Services.AddTransient<CashDetailPageModel>();
            builder.Services.AddTransient<CashDetailPage>();


            // Dashboard
            builder.Services.AddSingleton<DashboardService>();
            //TODO: for time being it is here it should be move to shared module

            builder.Services.AddSingleton<DefaultDashboardPageModel>();
            builder.Services.AddSingleton<DefaultDashboardPageModel>();

            builder.Services.AddSingleton<DashboardPage>();

           // builder.EnableDashboard();

          

            return builder;
        }

        public static void EnableCoreModulesRoutes()
        {
            //Register RouterH

            RouterHelper.AddRoute(typeof(DayBeginEntyPage));
            RouterHelper.AddRoute(typeof(DayEndEntryPage));
            RouterHelper.AddRoute(typeof(PettyCashSheetEntryPage));
            RouterHelper.AddRoute(typeof(EntryCashDetailPage));
             RouterHelper.AddRoute(typeof(EntryPettyCashSheetPage));
            AccountingModule.EnableAccountingRoutes();
            
            ClientModule.RegisterStoreRoutes();
            
            
        }

    }
}
