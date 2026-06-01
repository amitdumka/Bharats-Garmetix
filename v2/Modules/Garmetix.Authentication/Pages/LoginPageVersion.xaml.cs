namespace Garmetix.Authentication.Pages;

public partial class LoginPageVersion : ContentPage
{
	public LoginPageVersion()
	{
		InitializeComponent();
	}
    private bool _isPasswordHidden = true;

    

    private void OnEyeIconTapped(object sender, TappedEventArgs e)
    {
        // Toggle password visibility
        _isPasswordHidden = !_isPasswordHidden;
        PasswordEntry.IsPassword = _isPasswordHidden;

        // Optionally, update the eye icon image here
        // var image = (Image)sender;
        // image.Source = _isPasswordHidden ? "eye_icon.png" : "eye_off_icon.png";
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Add your authentication logic here
        await DisplayAlert("Login", "Attempting to log into Aadwika Fashion...", "OK");
    }

    private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {
        // Add your navigation logic here
        await DisplayAlert("Forgot Password", "Navigate to reset password page.", "OK");
    }
}