using Garmetix.Onboarding.ViewModels;

namespace Garmetix.Onboarding.Pages;

public partial class Step1BasicInformationPage : ContentPage
{
    public Step1BasicInformationPage(Step1BasicInfoViewModel viewModel)
    {
        InitializeComponent();
        viewModel.DataForm = clientDetailsForm;
        BindingContext = viewModel;

    }
}