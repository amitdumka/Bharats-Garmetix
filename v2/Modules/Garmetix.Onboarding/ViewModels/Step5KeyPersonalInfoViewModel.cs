using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Onboarding.Models;
using Garmetix.Onboarding.Services;

namespace Garmetix.Onboarding.ViewModels
{
    public partial class Step5KeyPersonalInfoViewModel : OnboardingBaseViewModel
    {
        [ObservableProperty]
        private KeyPersonalInfo? _keyPersonalInfo;

        public Step5KeyPersonalInfoViewModel(OnboardingStateService onboardingStateService) : base(onboardingStateService)
        {
            Title = "Step 5: Key Personal Information";
        }

        protected override void LoadDataFromState()
        {
            this.KeyPersonalInfo = _onboardingStateService.CurrentOnboardingData.KeyPersonalDetails;
            // Load the KeyPersonalInfo from the state service
            // This will be used to pre-populate the form fields if needed
        }

        protected override async Task NextStep()
        {
            if (!ValidateProperties())
            {
                return;
            }

            IsBusy = true;
            // Save data to the state service
            _onboardingStateService.CurrentOnboardingData.KeyPersonalDetails = this.KeyPersonalInfo; // Save the entire ClientInfo object
            // Simulate some work
            await Task.Delay(500);
            await Shell.Current.GoToAsync("ReviewPage");
            IsBusy = false;
        }

        //protected override bool ValidateProperties()
        //{
        //    ClearErrors();
        //    ValidateAllProperties();
        //    _dataForm.Commit();
        //    var result = _dataForm.Validate();
        //    if (HasErrors || !result)
        //    {
        //        _ = Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation Error", "Please correct the highlighted fields.", "OK");
        //        return false;
        //    }
        //    return true;
        //}
    }
}