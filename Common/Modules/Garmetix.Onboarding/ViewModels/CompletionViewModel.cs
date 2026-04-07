using CommunityToolkit.Mvvm.Input;
using Garmetix.Onboarding.Services;


namespace Garmetix.Onboarding.ViewModels
{
    public partial class CompletionViewModel : BaseViewModel
    {
        private readonly OnboardingStateService _onboardingStateService;
        public CompletionViewModel(OnboardingStateService onboardingStateService)
        {
            Title = "Onboarding Complete!";
            _onboardingStateService = onboardingStateService;
        }

        [RelayCommand]
        async Task Finish()
        {
            // Reset onboarding data for next potential registration
            _onboardingStateService.ResetOnboardingData();

            // Navigate to the main part of the app or login page
            // For this example, we'll go back to the initial step (or a hypothetical HomePage)
            // You might want to clear the navigation stack.
            // await Shell.Current.GoToAsync("//HomePage"); // If you have a HomePage
            await Shell.Current.GoToAsync("//home"); // Or back to start for demo
        }
    }
}
