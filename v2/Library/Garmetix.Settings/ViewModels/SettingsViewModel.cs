using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Garmetix.Settings.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy = false;

        // --- AUTHENTICATION ---
        [ObservableProperty] private bool isAutoLoginEnabled = false;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string password = string.Empty;

        // --- SESSION & STORE DATA ---
        [ObservableProperty] private string companyCode = string.Empty;
        [ObservableProperty] private string storeGroupCode = string.Empty;
        [ObservableProperty] private string storeCode = string.Empty;

        // --- CLOUD & NETWORK ---
        [ObservableProperty] private bool isOnlineMode = false;
        [ObservableProperty] private string apiUrl = string.Empty;
        [ObservableProperty] private string appUniqueId = string.Empty;
        [ObservableProperty] private bool isAutoBackupEnabled = false;

        // --- FEATURES & INTEGRATIONS ---
        [ObservableProperty] private bool isVoucherPrintEnabled = true;
        [ObservableProperty] private bool isInvoicePrintEnabled = true;
        [ObservableProperty] private bool isWhatsAppShareEnabled = true;

        // --- UI PREFERENCES ---
        [ObservableProperty] private bool isDarkMode = true;

        public SettingsViewModel()
        {
            // The unique ID should only be generated once per device installation
            AppUniqueId = Preferences.Default.Get("AppUniqueId", Guid.NewGuid().ToString().ToUpper());
            Preferences.Default.Set("AppUniqueId", AppUniqueId);
        }

        public async Task LoadSettingsAsync()
        {
            IsBusy = true;
            try
            {
                // Load Auth
                IsAutoLoginEnabled = Preferences.Default.Get("IsAutoLoginEnabled", false);
                Username = Preferences.Default.Get("Username", string.Empty);
                Password = await SecureStorage.Default.GetAsync("UserPassword") ?? string.Empty;

                // Load Store Data
                CompanyCode = Preferences.Default.Get("CompanyCode", "GMX");
                StoreGroupCode = Preferences.Default.Get("StoreGroupCode", "DEFAULT");
                StoreCode = Preferences.Default.Get("StoreCode", "AFA");

                // Load Cloud Data
                IsOnlineMode = Preferences.Default.Get("IsOnlineMode", true);
                ApiUrl = Preferences.Default.Get("ApiUrl", "https://api.garmetix.com/v1/");
                IsAutoBackupEnabled = Preferences.Default.Get("IsAutoBackupEnabled", true);

                // Load Features
                IsVoucherPrintEnabled = Preferences.Default.Get("IsVoucherPrintEnabled", true);
                IsInvoicePrintEnabled = Preferences.Default.Get("IsInvoicePrintEnabled", true);
                IsWhatsAppShareEnabled = Preferences.Default.Get("IsWhatsAppShareEnabled", true);

                // Load UI
                IsDarkMode = Preferences.Default.Get("IsDarkMode", false);
            }
            catch (Exception ex)
            {
                var page = Application.Current?.Windows[0].Page;
                if (page != null)
                {
                    await page.DisplayAlertAsync("Error", $"Failed to load settings: {ex.Message}", "OK");
                }
                else
                {
                    Debug.WriteLine($"Failed to load settings: {ex}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveSettingsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                // Save Auth
                Preferences.Default.Set("IsAutoLoginEnabled", IsAutoLoginEnabled);
                Preferences.Default.Set("Username", Username);

                if (!string.IsNullOrWhiteSpace(Password))
                    await SecureStorage.Default.SetAsync("UserPassword", Password);
                else
                    SecureStorage.Default.Remove("UserPassword");

                // Save Store Data
                Preferences.Default.Set("CompanyCode", CompanyCode);
                Preferences.Default.Set("StoreGroupCode", StoreGroupCode);
                Preferences.Default.Set("StoreCode", StoreCode);

                // Save Cloud Data
                Preferences.Default.Set("IsOnlineMode", IsOnlineMode);
                Preferences.Default.Set("ApiUrl", ApiUrl);
                Preferences.Default.Set("IsAutoBackupEnabled", IsAutoBackupEnabled);

                // Save Features
                Preferences.Default.Set("IsVoucherPrintEnabled", IsVoucherPrintEnabled);
                Preferences.Default.Set("IsInvoicePrintEnabled", IsInvoicePrintEnabled);
                Preferences.Default.Set("IsWhatsAppShareEnabled", IsWhatsAppShareEnabled);

                // Save and Apply UI Theme immediately
                Preferences.Default.Set("IsDarkMode", IsDarkMode);

                var app = Application.Current;
                if (app != null)
                {
                    app.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
                }

                var page = Application.Current?.Windows[0].Page;
                if (page != null)
                {
                    await page.DisplayAlertAsync("Success", "Settings saved successfully.", "OK");
                }
                else
                {
                    Debug.WriteLine("Settings saved successfully.");
                }
            }
            catch (Exception ex)
            {
                var page = Application.Current?.Windows[0].Page;
                if (page != null)
                {
                    await page.DisplayAlertAsync("Error", $"Failed to save settings: {ex.Message}", "OK");
                }
                else
                {
                    Debug.WriteLine($"Failed to save settings: {ex}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ResetToDefaultsAsync()
        {
            // Try to get a Page to show the confirmation dialog: prefer first window page, fallback to MainPage.
            Page? page = null;
            var app = Application.Current;
            if (app != null)
            {
                if (app.Windows != null && app.Windows.Count > 0)
                {
                    page = app.Windows[0].Page;
                }

                if (page == null)
                {
                    page = app.MainPage;
                }
            }

            if (page == null)
            {
                // No UI available to confirm; abort reset.
                Debug.WriteLine("ResetToDefaultsAsync aborted: no Page available for user confirmation.");
                return;
            }

            bool confirm = await page.DisplayAlertAsync("Reset Settings", "Are you sure you want to reset all settings to their default values?", "Yes, Reset", "Cancel");
            if (!confirm) return;

            Preferences.Default.Clear();
            SecureStorage.Default.RemoveAll();

            await LoadSettingsAsync();
        }
    }
}