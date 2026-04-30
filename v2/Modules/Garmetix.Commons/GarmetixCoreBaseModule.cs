using Garmetix.Accounting;
using Garmetix.Commons.DayOperations.PageModels;
using Garmetix.CoreBase.Dashboard;
using Garmetix.CoreBase.DayOperations.Pages;
using Garmetix.CoreBase.TimeLines;
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

            builder.UseAccounting().UseBanking().EnableDashboard().UseStores();

          

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
