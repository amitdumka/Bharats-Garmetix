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
                await DisplayAlertAsync("Error", "Please select a seed option.", "OK");
                return;
            }
            if (selectedSeed == "Aadwika Fashion By Amit Kumar")
            {
                // add try catch finally block here to handle any exceptions that may occur during database seeding
                try
                {
                    await DatabaseService.SeedDatabaseAsync("Aadwika Fashion By Amit Kumar");
                    Preferences.Set("IsOnboardingComplete", true);
                    await DisplayAlertAsync("Success", "Database seeded successfully!", "OK");
                    Application.Current?.Windows[0].Page = LoginPage;
                }
                catch (Exception ex)
                {

                    await DisplayAlertAsync("Error", $"An error occurred while seeding the database: {ex.Message}", "OK");
                    return;
                }
                
            }
            else if (selectedSeed == "Aadwika Fashion By Shalini Kumari")
            {
                // add try catch finally block here to handle any exceptions that may occur during database seeding
                try
                {
                    await DatabaseService.SeedDatabaseAsync("Aadwika Fashion By Shalini Kumari");
                    Preferences.Set("IsOnboardingComplete", true);
                    await DisplayAlertAsync("Success", "Database seeded successfully!", "OK");
                    Application.Current?.Windows[0].Page = LoginPage;
                }
                catch (Exception ex)
                {
                    await DisplayAlertAsync("Error", $"An error occurred while seeding the database: {ex.Message}", "OK");
                    return;
                }
                finally
                {
                    Application.Current?.Windows[0].Page = LoginPage;
                }
                
            }

            await DisplayAlertAsync("Error", "Database not seeded. Please try again.", "OK"); 
            //await DisplayAlertAsync("Success", "Database seeded successfully!", "OK");

        }
    }
}