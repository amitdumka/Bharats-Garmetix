using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.AI.Billing.Services;

namespace Garmetix.AI.Billing.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] private string username;
        [ObservableProperty] private string password;
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string errorMessage;

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both Username and Password.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            bool success = await AuthService.Instance.AuthenticateAsync(Username, Password);
            IsBusy = false;

            if (success)
            {
                // Go to PIN Setup to create the quick unlock code
                await Shell.Current.GoToAsync("//PinSetupPage");
            }
            else
            {
                ErrorMessage = "Invalid Username or Password.";
            }
        }
    }
}