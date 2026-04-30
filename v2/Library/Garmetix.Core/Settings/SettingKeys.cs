using Garmetix.Core.Enums;
using Garmetix.Core.Sessions;
using Microsoft.Extensions.Options;

namespace Garmetix.Core.Settings
{
    public static class SettingKeys
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

        public const string EnableMultiStoreKey = "EnableMultiStore";
        public const string WorkingModeKey = "WorkingMode";

        public const string AppIdKey = "AppId";
        public const string AppModeKey = "AppMode";

        public const string EnableAutoLoginKey = "EnableAutoLogin";
        public const string EnablePrintingKey = "EnablePrinting";
    }
    public class SettingsServices(IOptions<AppSettings> appSettings)
    {

        protected readonly IOptions<AppSettings> _appSettings = appSettings;
        public string GetRemoteUrl() => _appSettings.Value.RemoteUrl;

        //TODO: Implement GUID conversion for AppIdKey
        public AppSettings LoadSettings()
        {
            //string modeText = AppMode.Standalone.ToReadableString(); // "Standalone"
            // AppMode modeEnum = "Remote".ToEnum<AppMode>(); // AppMode.Remote
            return new AppSettings
            {
                EnableRemoteSync = Preferences.Get(SettingKeys.EnableRemoteSyncKey, false),
                RemoteUrl = Preferences.Get(SettingKeys.RemoteUrlKey, GetRemoteUrl()),
                EnableLocalCache = Preferences.Get(SettingKeys.EnableLocalCacheKey, false),
                EnableRemoteCache = Preferences.Get(SettingKeys.EnableRemoteCacheKey, false),

                CompanyName = Preferences.Get(SettingKeys.CompanyNameKey, string.Empty),
                GSTIN = Preferences.Get(SettingKeys.GSTINKey, string.Empty),

                // Cast integer stored back into the enum
                WorkingMode = (WorkingMode)Preferences.Get(SettingKeys.WorkingModeKey, (int)WorkingMode.Company),
                AppId = Guid.Parse(Preferences.Get(SettingKeys.AppIdKey, Guid.Empty.ToString())),
                AppMode = (AppMode)Preferences.Get(SettingKeys.AppModeKey, (int)AppMode.Standalone),

                EnableAutoLogin = Preferences.Get(SettingKeys.EnableAutoLoginKey, false),
                EnableMultiStore = Preferences.Get(SettingKeys.EnableMultiStoreKey, false),
                EnablePrinting = Preferences.Get(SettingKeys.EnablePrintingKey, true),
                // Company Store Code can be added if needed
                StoreName = Preferences.Get(SettingKeys.StoreNameKey, string.Empty),
                StoreAddress = Preferences.Get(SettingKeys.StoreAddressKey, string.Empty),
                StoreCity = Preferences.Get(SettingKeys.StoreCityKey, string.Empty),
                StoreState = Preferences.Get(SettingKeys.StoreStateKey, string.Empty),
                StorePincode = Preferences.Get(SettingKeys.StorePinCodeKey, string.Empty),
                StoreContact = Preferences.Get(SettingKeys.StorePhoneKey, string.Empty),
                StoreEmail = Preferences.Get(SettingKeys.StoreEmailKey, string.Empty),
                GroupName = Preferences.Get(SettingKeys.GroupNameKey, string.Empty),
                CompanyStoreCode = Preferences.Get(SettingKeys.CompanyStoreCodeKey, string.Empty)

            };
        }

        public static bool IsPrintingEnabled() => Preferences.Get(SettingKeys.EnablePrintingKey, false);

        public static void SaveSettings(AppSettings settings)
        {
            Preferences.Set(SettingKeys.EnableRemoteSyncKey, settings.EnableRemoteSync);
            Preferences.Set(SettingKeys.RemoteUrlKey, settings.RemoteUrl ?? string.Empty);
            Preferences.Set(SettingKeys.EnableLocalCacheKey, settings.EnableLocalCache);
            Preferences.Set(SettingKeys.EnableRemoteCacheKey, settings.EnableRemoteCache);
            Preferences.Set(SettingKeys.CompanyNameKey, settings.CompanyName ?? string.Empty);
            Preferences.Set(SettingKeys.GSTINKey, settings.GSTIN ?? string.Empty);
            Preferences.Set(SettingKeys.WorkingModeKey, (int)settings.WorkingMode);
            Preferences.Set(SettingKeys.AppIdKey, settings.AppId.ToString() ?? Guid.Empty.ToString());
            Preferences.Set(SettingKeys.AppModeKey, (int)settings.AppMode);
            Preferences.Set(SettingKeys.EnableMultiStoreKey, settings.EnableMultiStore);
            Preferences.Set(SettingKeys.EnablePrintingKey, settings.EnablePrinting);
            Preferences.Set(SettingKeys.StoreNameKey, settings.StoreName ?? string.Empty);
            Preferences.Set(SettingKeys.StoreAddressKey, settings.StoreAddress ?? string.Empty);
            Preferences.Set(SettingKeys.StoreCityKey, settings.StoreCity ?? string.Empty);
            Preferences.Set(SettingKeys.StoreStateKey, settings.StoreState ?? string.Empty);
            Preferences.Set(SettingKeys.StorePinCodeKey, settings.StorePincode ?? string.Empty);
            Preferences.Set(SettingKeys.StorePhoneKey, settings.StoreContact ?? string.Empty);
            Preferences.Set(SettingKeys.StoreEmailKey, settings.StoreEmail ?? string.Empty);
            Preferences.Set(SettingKeys.GroupNameKey, settings.GroupName ?? string.Empty);
            Preferences.Set(SettingKeys.CompanyStoreCodeKey, settings.CompanyStoreCode ?? string.Empty);



            UpdateSession(settings.EnableAutoLogin);
        }
        private static void UpdateSession(bool enableAutoLogin)
        {
            if (Preferences.Get(SettingKeys.EnableAutoLoginKey, false) != enableAutoLogin)
            {
                SessionService.CurrentSession.IsAutoLoginEnabled = enableAutoLogin;
                SessionService.UpdateSession();

            }
            Preferences.Set(SettingKeys.EnableAutoLoginKey, enableAutoLogin);

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