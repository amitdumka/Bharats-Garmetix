using Garmetix.Onboarding.ViewModels;

namespace Garmetix.Onboarding.Pages;

public partial class Step4CompanyConfigPage : ContentPage
{
    public Step4CompanyConfigPage(Step4CompanyConfigInfoViewModel viewModel)
    {
        InitializeComponent();
        viewModel.DataForm = clientDetailsForm;
        BindingContext = viewModel;
    }
}