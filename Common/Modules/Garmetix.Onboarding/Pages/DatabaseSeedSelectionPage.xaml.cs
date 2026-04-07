using Garmetix.Authentication.Pages;
using Garmetix.Databases.Services;

namespace Garmetix.Onboarding.Pages
{
    public partial class DatabaseSeedSelectionPage : ContentPage
    {
        private Login LoginPage;
        public DatabaseSeedSelectionPage(Login login)
        {
            InitializeComponent();
            LoginPage = login;
        }

        private async void OnSeedClicked(object sender, EventArgs e)
        {
            var selectedSeed = SeedPicker.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedSeed))
            {
                await DisplayAlert("Error", "Please select a seed option.", "OK");
                return;
            }
            if (selectedSeed == "Aadwika Fashion By Amit Kumar")
            {
                await DatabaseService.SeedDatabaseAsync("Aadwika Fashion By Amit Kumar");
                Preferences.Set("IsOnboardingComplete", true);
                await DisplayAlert("Success", "Database seeded successfully!", "OK");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS0618 // Type or member is obsolete
                Application.Current.MainPage = LoginPage;
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            else if (selectedSeed == "Aadwika Fashion By Shalini Kumari")
            {
                await DatabaseService.SeedDatabaseAsync("Aadwika Fashion By Shalini Kumari");
                Preferences.Set("IsOnboardingComplete", true);
                await DisplayAlert("Success", "Database seeded successfully!", "OK");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS0618 // Type or member is obsolete
                Application.Current.MainPage = LoginPage;
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            await DisplayAlert("Error", "Database not seeded. Please try again.", "OK"); await DisplayAlert("Success", "Database seeded successfully!", "OK");

        }
    }
}