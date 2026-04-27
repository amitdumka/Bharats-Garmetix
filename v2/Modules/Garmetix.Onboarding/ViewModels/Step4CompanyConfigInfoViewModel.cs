using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Onboarding.Models;
using Garmetix.Onboarding.Services;

namespace Garmetix.Onboarding.ViewModels
{
    public partial class Step4CompanyConfigInfoViewModel : OnboardingBaseViewModel
    {
        [ObservableProperty]
        private CompanyConfigInfo? _companyConfigInfo;
        public Step4CompanyConfigInfoViewModel(OnboardingStateService onboardingStateService) : base(onboardingStateService)
        {
            Title = "Step 4: Company Configuration Information";

        }

        protected override void LoadDataFromState()
        {
            this.CompanyConfigInfo = _onboardingStateService.CurrentOnboardingData.CompanyConfig;
        }

        protected override async Task NextStep()
        {
            if (!ValidateProperties())
            {
                return;
            }

            IsBusy = true;
            // Save data to the state service
            _onboardingStateService.CurrentOnboardingData.CompanyConfig = this.CompanyConfigInfo; // Save the entire ClientInfo object
            // Simulate some work
            await Task.Delay(500);
            await Shell.Current.GoToAsync("Step5KeyPersonalInfoPage");
            IsBusy = false;
        }


    }
}
