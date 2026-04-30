using Bharat.ToolKits.Helpers;
using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.CoreBase.Stores.Pages;
using Garmetix.CoreBase.Stores.Pages.Desktop;
using Garmetix.CoreBase.Stores.Pages.Mobile;

namespace Garmetix.Stores
{
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

        public static void RegisterStoreRoutes()
        {
            RouterHelper.AddRoute(typeof(EntryCompanyPage));
            RouterHelper.AddRoute(typeof(EntryStorePage));
            RouterHelper.AddRoute(typeof(EntryStoreGroupPage));
        }
    }
}