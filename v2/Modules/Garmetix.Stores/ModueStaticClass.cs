
using Bharat.ToolKits.Helpers;
using Garmetix.Base.Shells;
using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.CoreBase.Stores.Pages;
using Garmetix.CoreBase.Stores.Pages.Desktop;
using Garmetix.CoreBase.Stores.Pages.Mobile;

namespace Garmetix.Stores
{

    public partial class CompanyMenu : BaseFlyoutMenu
    {
        public CompanyMenu() : base("Stores")
        {
            // --- Platform Specific Tabs ---
            AddPlatformSpecificPageTab("Store", "rain_icon.png", "Stores",
                mobilePageType: typeof(StorePage),
                desktopPageType: typeof(StoresPage));

            AddPlatformSpecificPageTab("Store Group", "rain_icon.png", "Group",
                mobilePageType: typeof(StoreGroupPage),
                desktopPageType: typeof(StoreGroupsPage));

            AddPlatformSpecificPageTab("Company", "rain_icon.png", "Client",
                mobilePageType: typeof(CompanyPage),
                desktopPageType: typeof(CompaniesPage));

            // --- Standard Tabs ---
            // AddPageTab("Day Begin", "rain_icon.png", "daybegib", typeof(DayBeginEntyPage));
            // AddPageTab("Day Closing", "rain_icon.png", "dayend", typeof(DayEndEntryPage));
            // AddPageTab("Petty Cash Sheet", "rain_icon.png", "cashsheet", typeof(PettyCashSheetEntryPage));
        }
    }

    public static class ClientModule
    {


        public static MauiAppBuilder UseStores(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<CompanyPageModel>();
            builder.Services.AddSingleton<CompaniesPage>();
            builder.Services.AddSingleton<CompanyPage>();

            builder.Services.AddTransient<CompanyFormModel>();
            builder.Services.AddTransient<EntryCompanyPage>();



            builder.Services.AddSingleton<StoreGroupPageModel>();
            builder.Services.AddSingleton<StoreGroupsPage>();
            builder.Services.AddSingleton<StoreGroupPage>();

            builder.Services.AddTransient<StoreGroupFormModel>();
            builder.Services.AddTransient<EntryStoreGroupPage>();

            builder.Services.AddSingleton<StorePageModel>();
            builder.Services.AddSingleton<StorePage>();
            builder.Services.AddSingleton<StoresPage>();

            builder.Services.AddTransient<StoreFormModel>();
            builder.Services.AddTransient<EntryStorePage>();

            return builder;
        }

        public static void EnableStoreRoutes()
        {
            RouterHelper.AddRoute(typeof(EntryCompanyPage));
            RouterHelper.AddRoute(typeof(EntryStorePage));
            RouterHelper.AddRoute(typeof(EntryStoreGroupPage));
        }
    }
}
