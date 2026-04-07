using Garmetix.Authentication.Pages;
using Garmetix.Onboarding.Services;
using Garmetix.Onboarding.ViewModels;

namespace Garmetix.Onboarding.Pages
{
    public partial class OnboardingChoicePage : ContentPage
    {
        private readonly OnboardingStateService _onboardingStateService = OnboardingStateService.Instance;
        public static Login LoginPage { get; private set; }
        public OnboardingChoicePage(Login page)
        {
            InitializeComponent();
            LoginPage = page;
        }

        private async void OnStartOnboardingClicked(object sender, EventArgs e)
        {
            // Create the required ViewModel instance and pass the required service
            var viewModel = new Step1BasicInfoViewModel(_onboardingStateService);
            await Navigation.PushAsync(new Step1BasicInformationPage(viewModel));
        }

        private async void OnSeedDatabaseClicked(object sender, EventArgs e)
        {
            // Navigate to a page where user can select from a list to seed the database
            await Navigation.PushAsync(new DatabaseSeedSelectionPage(LoginPage));
        }
    }
}