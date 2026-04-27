using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Onboarding.Models;
using Garmetix.Onboarding.Services;

namespace Garmetix.Onboarding.ViewModels
{
    public partial class Step1BasicInfoViewModel : OnboardingBaseViewModel
    {
        [ObservableProperty]
        protected ClientInfo clientDetails;

        public Step1BasicInfoViewModel(OnboardingStateService onboardingStateService) : base(onboardingStateService)
        {
            Title = "Step 1: Basic Information";
            isStep1 = true;
        }

        protected override void LoadDataFromState()
        {
            clientDetails = _onboardingStateService.CurrentOnboardingData.ClientDetails;
        }

        protected override async Task NextStep()
        {
            if (!ValidateProperties())
            {
                return;
            }

            IsBusy = true;
            // Save data to the state service
            _onboardingStateService.CurrentOnboardingData.ClientDetails = clientDetails; // Save the entire ClientInfo object
            // Simulate some work
            await Task.Delay(500);

            await Shell.Current.GoToAsync("Step2CompanyDetailsPage");
            IsBusy = false;
        }
    }
}