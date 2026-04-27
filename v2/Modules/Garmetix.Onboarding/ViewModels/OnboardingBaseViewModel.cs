using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Onboarding.Services;
using Syncfusion.Maui.DataForm;
using System.ComponentModel.DataAnnotations;


namespace Garmetix.Onboarding
{
    public abstract partial class OnboardingBaseViewModel : BaseViewModel
    {
        protected readonly OnboardingStateService _onboardingStateService;
        protected SfDataForm _dataForm;
        [ObservableProperty]
        public bool isStep1 = false;

        public SfDataForm DataForm { get => _dataForm; set => _dataForm = value; }
        public OnboardingBaseViewModel(OnboardingStateService onboardingStateService)
        {
            _onboardingStateService = onboardingStateService;
            LoadDataFromState();
        }
        protected abstract void LoadDataFromState();
        // This method can be overridden in derived classes to load data from the state service
        // Example: var clientDetails = _onboardingStateService.CurrentOnboardingData.ClientDetails;

        protected virtual bool ValidateProperties()
        {
            // Clear previous errors
            ClearErrors();
            // Validate all properties
            ValidateAllProperties();

            _dataForm.Commit();
            // Validate the SfDataForm
            var result = _dataForm.Validate();
            // Check if there are any errors

            if (HasErrors || !result)
            {
                // Optionally, display a summary of errors or focus on the first invalid field
                // For simplicity, we'll just rely on the UI to show individual errors.
                // You might want to show a general error message.
                _ = Application.Current.MainPage.DisplayAlert("Validation Error", "Please correct the highlighted fields.", "OK");
                return false;
            }
            return true;
        }
        // This method can be overridden in derived classes to validate properties specific to the step
        [RelayCommand]
        async Task GoBack()
        {
            if (IsStep1)
            {
                return;
            }

            if (IsBusy)
            {
                return;
            }

            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        protected abstract Task NextStep();

        public static ValidationResult ValidateDateOfBith(DateTime DateOfBirth, ValidationContext context)
        {

            if (DateOfBirth == null)
            {
                return new ValidationResult("Date of birth is required.");
            }
            else if (DateOfBirth > DateTime.Today.AddYears(-16)) // Example: Must be at least 16
            {
                return new("You must be at least 16 years old.");
            }
            else
            {
                return ValidationResult.Success;
            }

        }

    }
}
