using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Garmetix.AI.Billing.Services;

namespace Garmetix.AI.Billing.ViewModels
{
    public partial class PinUnlockViewModel : ObservableObject
    {
        [ObservableProperty] private string pinEntry = "";
        [ObservableProperty] private string message = "Enter your 4-digit PIN";
        [ObservableProperty] private Color messageColor = Color.FromArgb("#64748B");

        // UI Dot Indicators (Filled or Empty)
        [ObservableProperty] private bool dot1;
        [ObservableProperty] private bool dot2;
        [ObservableProperty] private bool dot3;
        [ObservableProperty] private bool dot4;

        [RelayCommand]
        public async Task KeypressAsync(string digit)
        {
            Message = "Enter your 4-digit PIN";
            MessageColor = Color.FromArgb("#64748B");

            if (digit == "DEL")
            {
                if (PinEntry.Length > 0) PinEntry = PinEntry.Substring(0, PinEntry.Length - 1);
            }
            else if (PinEntry.Length < 4)
            {
                PinEntry += digit;
            }

            UpdateDots();

            if (PinEntry.Length == 4)
            {
                await ValidatePinAsync();
            }
        }

        private void UpdateDots()
        {
            Dot1 = PinEntry.Length >= 1;
            Dot2 = PinEntry.Length >= 2;
            Dot3 = PinEntry.Length >= 3;
            Dot4 = PinEntry.Length >= 4;
        }

        private async Task ValidatePinAsync()
        {
            bool isValid = await AuthService.Instance.ValidatePinAsync(PinEntry);

            if (isValid)
            {
                Message = "Access Granted";
                MessageColor = Color.FromArgb("#10B981");
                await Task.Delay(300); // Brief pause for UX

                // Navigate to the Main App Dashboard
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            else
            {
                Message = "Incorrect PIN. Try again.";
                MessageColor = Color.FromArgb("#EF4444");

                // Shake effect logic could go here
                PinEntry = "";
                UpdateDots();
            }
        }

        [RelayCommand]
        public async Task SwitchUserAsync()
        {
            AuthService.Instance.Logout();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}