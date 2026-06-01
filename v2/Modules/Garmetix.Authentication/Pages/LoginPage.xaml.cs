namespace Garmetix.Authentication.Pages;

public partial class LoginPage : ContentPage
{
    private bool _showPassword;
    public LoginPage()
    {
        InitializeComponent();
    }


    private void TogglePassword(object sender, EventArgs e) { _showPassword = !_showPassword; PasswordEntry.IsPassword = !_showPassword; }

    private async void LoginClicked(object sender, EventArgs e)
    {
        try
        {
            ((Button)sender).IsEnabled = false;
            // Call API Here
            await DisplayAlert("Garmetix", "Login Successful", "OK");
            await Shell.Current.GoToAsync("//Dashboard");
        }
        finally { ((Button)sender).IsEnabled = true; }
    }
}