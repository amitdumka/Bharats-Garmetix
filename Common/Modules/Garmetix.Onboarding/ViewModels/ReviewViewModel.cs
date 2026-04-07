using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Onboarding.Models;
using Garmetix.Onboarding.Services;


namespace Garmetix.Onboarding.ViewModels
{
    public partial class ReviewViewModel : BaseViewModel
    {
        private readonly OnboardingStateService _onboardingStateService;
        private readonly IOnboardingSubmissionService _submissionService;

        [ObservableProperty]
        OnboardingData dataToReview;

        public ReviewViewModel(OnboardingStateService onboardingStateService, IOnboardingSubmissionService submissionService)
        {
            Title = "Review Your Information";
            _onboardingStateService = onboardingStateService;
            _submissionService = submissionService;
            DataToReview = _onboardingStateService.CurrentOnboardingData;
        }

        [RelayCommand]
        async Task GoBack()
        {
            if (IsBusy)
            {
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task Submit()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            bool success = await _submissionService.SubmitOnboardingDataAsync(DataToReview);
            IsBusy = false;

            if (success)
            {
                // Optionally clear the onboarding state after successful submission
                // _onboardingStateService.ResetOnboardingData(); 
                await Shell.Current.GoToAsync("CompletionPage");
            }
            else
            {
                // Handle submission failure
                await Application.Current.MainPage.DisplayAlert("Submission Failed", "Could not submit your information. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task EditSection(string sectionPage)
        {
            if (string.IsNullOrWhiteSpace(sectionPage) || IsBusy)
            {
                return;
            }
            // Navigate directly to the specified edit page.
            // The routes should be defined in AppShell.
            // Example: "///Step1BasicInfoPage"
            await Shell.Current.GoToAsync($"///{sectionPage}");
        }
    }
}
