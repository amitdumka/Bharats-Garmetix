using Bharat.ToolKits.Helpers;
using Garmetix.Core.Enums;
using Garmetix.Core.Settings;  
using System.Text.Json;

namespace Garmetix.Core.Sessions
{
    public static class SessionService
    {
        //Handling App Session

        public static string? GstNumber()
        {
            return StorageOps.GetPref(SettingsService.GSTINKey, "");
        }

        public static string? GroupName()
        {
            return StorageOps.GetPref(SettingsService.GroupNameKey, "");
        }

        public static string? StoreName()
        {
            return StorageOps.GetPref(SettingsService.StoreNameKey, "");
        }

        [Obsolete("Use CompanyStoreCode() instead")]
        public static string CompanyCode => Preferences.Get("CompanyStoreCode", "DMY");

        public static string? CompanyStoreCode()
        {
            return StorageOps.GetPref(SettingsService.CompanyStoreCodeKey, "");
        }

        public static string? CompanyName()
        {
            return StorageOps.GetPref(SettingsService.CompanyNameKey, "");
        }

        public static string? StoreAddress()
        {
            return StorageOps.GetPref(SettingsService.StoreAddressKey, "");
        }

        public static string? Email()
        {
            return StorageOps.GetPref(SettingsService.StoreEmailKey, "amit.dumka@gmail.com");
        }

        public static string? Phone()
        {
            return StorageOps.GetPref(SettingsService.StorePhoneKey, "");
        }

        public static string? City()
        {
            return StorageOps.GetPref(SettingsService.StoreCityKey, "");
        }

        //Session Management
        public const string SessionKey = "AppSessionKey";

        public static CurrentSession? CurrentSession { get; private set; }
        public static bool IsUserLoggedIn => CurrentSession != null;

        public static Task UpdateSession()
        {
            return Task.Run(async delegate
            {
                try
                {
                    var sessionJson = JsonSerializer.Serialize(CurrentSession);
                    await SecureStorage.Default.SetAsync(SessionKey, sessionJson);
                }
                catch (Exception ex)
                {
                    // Log error or handle inability to save session
                    System.Diagnostics.Debug.WriteLine($"Error saving session to SecureStorage: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Start The Session /Login
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        public static Task StartSessionAsync(CurrentSession sessionInfo)
        {
            return Task.Run(async delegate
            {
                CurrentSession = sessionInfo;
                // Save to storage if autologinEnabled.
                if (sessionInfo.IsAutoLoginEnabled)
                {
                    try
                    {
                        var sessionJson = JsonSerializer.Serialize(sessionInfo);
                        await SecureStorage.Default.SetAsync(SessionKey, sessionJson);
                    }
                    catch (Exception ex)
                    {
                        // Log error or handle inability to save session
                        System.Diagnostics.Debug.WriteLine($"Error saving session to SecureStorage: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// End the session / Logout
        /// </summary>
        /// <returns></returns>
        public static async Task EndSessionAsync()
        {
            CurrentSession = null;
            SecureStorage.Default.Remove(SessionKey);
            // Potentially clear other related keys if any
            await Task.CompletedTask; // If no other async operations needed
        }

        public static Task<string> Get_Secure_Preference(string key)
        {
            return Task.Run(async delegate
            {
                string returnString = "";
                string? pref = await SecureStorage.GetAsync(key);
                if (pref != null)
                {
                    returnString = pref;
                }
                return returnString;
            });
        }

        /// <summary>
        /// If AutoLogin is enabled
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> TryLoadSessionAsync()
        {
            try
            {
                var sessionJson = Get_Secure_Preference(SessionKey).Result;
                if (!string.IsNullOrEmpty(sessionJson))
                {
                    CurrentSession = JsonSerializer.Deserialize<CurrentSession>(sessionJson);

                    if (CurrentSession != null)
                    {
                        // Optional: Validate session (e.g., token expiry if using tokens)
                        if (!CurrentSession.IsAutoLoginEnabled)
                        {
                            return false;
                        }
                        //TODO: what is can doCurrentSession.LoadFromXaml(SessionKey);

                        if (DateTime.Now > CurrentSession.LoginTime.AddHours(12))
                        {
                            CurrentSession.LoginTime = DateTime.Now;
                        }

                        System.Diagnostics.Debug.WriteLine($"Session loaded for user: {CurrentSession.UserName}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading session from SecureStorage: {ex.Message}");
                SecureStorage.Default.Remove(SessionKey); // Clear corrupted session data
            }
            return false;
        }

        /// <summary>
        /// Clear Session
        /// </summary>
        public static void ClearSession()
        {
            CurrentSession = null;
            SecureStorage.Default.Remove(SessionKey);
            // Clear other session-related data if needed
        }

        // Role-based access check helpers

        public static bool IsAdmin() => IsUserLoggedIn && CurrentSession.UserRole == LoginRole.Admin;

        public static bool IsAdminUser => IsUserLoggedIn && CurrentSession.UserRole == LoginRole.Admin;

        public static bool CanEditRecords() => IsUserLoggedIn &&
            (CurrentSession.UserRole == LoginRole.Admin || CurrentSession.UserRole == LoginRole.PowerUser || CurrentSession.UserRole == LoginRole.Accountant);

        public static bool CanDeleteRecords() => IsUserLoggedIn &&
            (CurrentSession.UserRole == LoginRole.Admin || CurrentSession.UserRole == LoginRole.PowerUser);

        public static bool CanAddOrSaveRecords() => IsUserLoggedIn &&
                (CurrentSession.UserRole == LoginRole.Admin || CurrentSession.UserRole == LoginRole.PowerUser || CurrentSession.UserRole == LoginRole.StoreManager || CurrentSession.UserRole == LoginRole.Accountant);

        public static bool CanViewRecords() => IsUserLoggedIn; // All logged-in users can view their store's data

        public static bool CanViewReports() => IsUserLoggedIn &&
            (CurrentSession.UserRole == LoginRole.Admin || CurrentSession.UserRole == LoginRole.PowerUser || CurrentSession.UserRole == LoginRole.StoreManager || CurrentSession.UserRole == LoginRole.Accountant);

        // Call this to refresh any UI bound to these role checks if roles could change dynamically (rare)
        // Or more commonly, when BaseViewModel properties are initialized or updated.
        public static event Action OnSessionChanged;

        public static void NotifySessionChanged() => OnSessionChanged?.Invoke();
    }
}