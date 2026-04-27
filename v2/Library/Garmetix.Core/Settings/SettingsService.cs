using Garmetix.Core.Sessions;
using Microsoft.Extensions.Options;

namespace Garmetix.Core.Settings
{
    //TODO: Note: This class was static in first instance, it was converted into singleton
    /// <summary>
    /// Settings Service for App
    /// Possible to Make it shareable
    /// </summary>
    public class SettingsService(IOptions<AppSettings> appSettings)
    {
        // Define keys for each setting
        public const string EnableRemoteSyncKey = "EnableRemoteSync";

        public const string RemoteUrlKey = "RemoteUrl";
        public const string EnableLocalCacheKey = "EnableLocalCache";
        public const string EnableRemoteCacheKey = "EnableRemoteCache";

        //Company Details
        public const string CompanyNameKey = "CompanyName";
        public const string GSTINKey = "GSTIN";
        public const string CompanyStoreCodeKey = "StoreCode";

        // Group Details
        public const string GroupNameKey = "GroupName";

        //Store Details
        public const string StoreNameKey = "StoreName";
        public const string StoreAddressKey = "StoreAddress";
        public const string StoreCityKey = "StoreCity";
        public const string StoreStateKey = "StoreState";
        public const string StorePinCodeKey = "StorePinCode";
        public const string StorePhoneKey = "StorePhone";
        public const string StoreEmailKey = "StoreEmail";



        public const string WorkingModeKey = "WorkingMode";

        public const string AppIdKey = "AppId";
        public const string AppModeKey = "AppMode";

        public const string EnableAutoLoginKey = "EnableAutoLogin";
        public const string EnablePrintingKey = "EnablePrinting";

        private readonly IOptions<AppSettings> _appSettings = appSettings;

        public string GetRemoteUrl() => _appSettings.Value.RemoteUrl;

        //TODO: Implement GUID conversion for AppIdKey
        public AppSettings LoadSettings()
        {
            //string modeText = AppMode.Standalone.ToReadableString(); // "Standalone"
            // AppMode modeEnum = "Remote".ToEnum<AppMode>(); // AppMode.Remote
            return new AppSettings
            {
                EnableRemoteSync = Preferences.Get(EnableRemoteSyncKey, false),
                RemoteUrl = Preferences.Get(RemoteUrlKey, GetRemoteUrl()),
                EnableLocalCache = Preferences.Get(EnableLocalCacheKey, false),
                EnableRemoteCache = Preferences.Get(EnableRemoteCacheKey, false),

                CompanyName = Preferences.Get(CompanyNameKey, string.Empty),
                GSTIN = Preferences.Get(GSTINKey, string.Empty),

                // Cast integer stored back into the enum
                WorkingMode = (WorkingMode)Preferences.Get(WorkingModeKey, (int)WorkingMode.Company),
                AppId = Guid.Parse(Preferences.Get(AppIdKey, Guid.Empty.ToString())),
                AppMode = (AppMode)Preferences.Get(AppModeKey, (int)AppMode.Standalone),

                EnableAutoLogin = Preferences.Get(EnableAutoLoginKey, false),
                EnableMultiStore = Preferences.Get("EnableMultiStore", false),
                EnablePrinting = Preferences.Get(EnablePrintingKey, true),
                // Company Store Code can be added if needed
                StoreName = Preferences.Get(StoreNameKey, string.Empty),
                StoreAddress = Preferences.Get(StoreAddressKey, string.Empty),
                StoreCity = Preferences.Get(StoreCityKey, string.Empty),
                StoreState = Preferences.Get(StoreStateKey, string.Empty),
                StorePincode = Preferences.Get(StorePinCodeKey, string.Empty),
                StoreContact = Preferences.Get(StorePhoneKey, string.Empty),
                StoreEmail = Preferences.Get(StoreEmailKey, string.Empty),
                GroupName = Preferences.Get(GroupNameKey, string.Empty),
                CompanyStoreCode = Preferences.Get(CompanyStoreCodeKey, string.Empty)


            };
        }

        public static bool IsPrintingEnabled() => Preferences.Get(EnablePrintingKey, false);

        public static void SaveSettings(AppSettings settings)
        {
            Preferences.Set(EnableRemoteSyncKey, settings.EnableRemoteSync);
            Preferences.Set(RemoteUrlKey, settings.RemoteUrl ?? string.Empty);
            Preferences.Set(EnableLocalCacheKey, settings.EnableLocalCache);
            Preferences.Set(EnableRemoteCacheKey, settings.EnableRemoteCache);
            Preferences.Set(CompanyNameKey, settings.CompanyName ?? string.Empty);
            Preferences.Set(GSTINKey, settings.GSTIN ?? string.Empty);
            Preferences.Set(WorkingModeKey, (int)settings.WorkingMode);
            Preferences.Set(AppIdKey, settings.AppId.ToString() ?? Guid.Empty.ToString());
            Preferences.Set(AppModeKey, (int)settings.AppMode);
            Preferences.Set("EnableMultiStore", settings.EnableMultiStore);
            Preferences.Set(EnablePrintingKey, settings.EnablePrinting);

            Preferences.Set(StoreNameKey, settings.StoreName ?? string.Empty);
            Preferences.Set(StoreAddressKey, settings.StoreAddress ?? string.Empty);
            Preferences.Set(StoreCityKey, settings.StoreCity ?? string.Empty);
            Preferences.Set(StoreStateKey, settings.StoreState ?? string.Empty);
            Preferences.Set(StorePinCodeKey, settings.StorePincode ?? string.Empty);
            Preferences.Set(StorePhoneKey, settings.StoreContact ?? string.Empty);
            Preferences.Set(StoreEmailKey, settings.StoreEmail ?? string.Empty);
            Preferences.Set(GroupNameKey, settings.GroupName ?? string.Empty);
            Preferences.Set(CompanyStoreCodeKey, settings.CompanyStoreCode ?? string.Empty);



            UpdateSession(settings.EnableAutoLogin);
        }
        private static void UpdateSession(bool enableAutoLogin)
        {
            if (Preferences.Get(EnableAutoLoginKey, false) != enableAutoLogin)
            {
                SessionService.CurrentSession.IsAutoLoginEnabled = enableAutoLogin;
                SessionService.UpdateSession();

            }
            Preferences.Set(EnableAutoLoginKey, enableAutoLogin);

        }


        public static void SetCompanyInfo(string companyName, string gstin, string storeCode, string groupName)
        {
            Preferences.Set("CompanyName", companyName);
            Preferences.Set("GSTIN", gstin);
            Preferences.Set("StoreCode", storeCode);
            Preferences.Set("GroupName", groupName);
        }
        public static void SetStoreInfo(string storeName, string storeAddress, string storeCity, string storeState, string storePincode, string storeEmail, string storeContact)
        {
            Preferences.Set("StoreName", storeName);
            Preferences.Set("StoreAddress", storeAddress);
            Preferences.Set("StoreCity", storeCity);
            Preferences.Set("StoreState", storeState);
            Preferences.Set("StorePinCode", storePincode);
            Preferences.Set("StoreEmail", storeEmail);
            Preferences.Set("StorePhone", storeContact);
        }

    }
}