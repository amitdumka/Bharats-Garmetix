using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Onboarding.Models;
using Garmetix.Onboarding.Services;

namespace Garmetix.Onboarding.ViewModels
{
    public partial class Step3AddressViewModel : OnboardingBaseViewModel
    {
        [ObservableProperty]
        private AddressInfo _addressDetails;

        public Step3AddressViewModel(OnboardingStateService onboardingStateService) : base(onboardingStateService)
        {
            Title = "Step 3: Address Information";
        }

        protected override void LoadDataFromState()
        {
            this.AddressDetails = _onboardingStateService.CurrentOnboardingData.AddressDetails;
        }

        protected override async Task NextStep()
        {
            if (!ValidateProperties())
            {
                return;
            }

            IsBusy = true;
            _onboardingStateService.CurrentOnboardingData.AddressDetails = this.AddressDetails;
            await Task.Delay(500); // Simulate work
            await Shell.Current.GoToAsync("Step4CompanyInfoPage");
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
        //        _=Application.Current.MainPage.DisplayAlert("Validation Error", "Please correct the highlighted fields.", "OK");
        //        return false;
        //    }
        //    return true;
        //}
    }
}