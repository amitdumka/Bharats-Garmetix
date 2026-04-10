using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Garmetix.Settings.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;

        // --- AUTHENTICATION ---
        [ObservableProperty] private bool isAutoLoginEnabled;
        [ObservableProperty] private string username;
        [ObservableProperty] private string password;

        // --- SESSION & STORE DATA ---
        [ObservableProperty] private string companyCode;
        [ObservableProperty] private string storeGroupCode;
        [ObservableProperty] private string storeCode;

        // --- CLOUD & NETWORK ---
        [ObservableProperty] private bool isOnlineMode;
        [ObservableProperty] private string apiUrl;
        [ObservableProperty] private string appUniqueId;
        [ObservableProperty] private bool isAutoBackupEnabled;

        // --- FEATURES & INTEGRATIONS ---
        [ObservableProperty] private bool isVoucherPrintEnabled;
        [ObservableProperty] private bool isInvoicePrintEnabled;
        [ObservableProperty] private bool isWhatsAppShareEnabled;

        // --- UI PREFERENCES ---
        [ObservableProperty] private bool isDarkMode;

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
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load settings: {ex.Message}", "OK");
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
                if (Application.Current != null)
                {
                    Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
                }

                await Application.Current.MainPage.DisplayAlert("Success", "Settings saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save settings: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ResetToDefaultsAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Reset Settings", "Are you sure you want to reset all settings to their default values?", "Yes, Reset", "Cancel");
            if (!confirm) return;

            Preferences.Default.Clear();
            SecureStorage.Default.RemoveAll();

            await LoadSettingsAsync();
        }
    }
}