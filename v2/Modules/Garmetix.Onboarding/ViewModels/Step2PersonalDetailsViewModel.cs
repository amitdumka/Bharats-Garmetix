using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Onboarding.Models;
using Garmetix.Onboarding.Services;

namespace Garmetix.Onboarding.ViewModels
{
    public partial class Step2CompanyDetailsViewModel : OnboardingBaseViewModel
    {
        [ObservableProperty]
        private CompanyInfo _companyInfo;

        public Step2CompanyDetailsViewModel(OnboardingStateService onboardingStateService) : base(onboardingStateService)
        {
            Title = "Step 2: Company Details";
        }

        protected override void LoadDataFromState()
        {
            _companyInfo = _onboardingStateService.CurrentOnboardingData.CompanyDetails;
        }

        //[RelayCommand]
        protected override async Task NextStep()
        {
            if (!ValidateProperties())
            {
                return;
            }

            IsBusy = true;

            await Task.Delay(500); // Simulate work

            await Shell.Current.GoToAsync("Step3AddressPage");
            IsBusy = false;
        }

        //protected override bool ValidateProperties()
        //{
        //    ClearErrors();
        //    ValidateAllProperties();
        //    // Commit the SfDataForm to ensure all changes are applied
        //    _dataForm.Commit(); // Ensure the SfDataForm is committed before validation
        //    try
        //    {
        //        var validationContext = new ValidationContext(CompanyInfo); // Use the generated property instead of the backing field
        //        Validator.ValidateObject(CompanyInfo, validationContext, true);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        // Handle validation exceptions, e.g., display an error message
        //        Application.Current.MainPage.DisplayAlert("Validation Error", ex.Message, "OK");
        //        return false;
        //    }
        //    // Validate the SfDataForm
        //    var result = _dataForm.Validate();
        //    // Check if there are any errors

        //    if (HasErrors || !result)
        //    {
        //        Application.Current.MainPage.DisplayAlert("Validation Error", "Please correct the highlighted fields.", "OK");
        //        return false;
        //    }
        //    return true;
        //}
    }
}