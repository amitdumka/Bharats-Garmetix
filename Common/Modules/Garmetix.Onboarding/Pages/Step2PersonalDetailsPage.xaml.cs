
using Garmetix.Onboarding.ViewModels;

namespace Garmetix.Onboarding.Pages;

public partial class Step2CompanyDetailsPage : ContentPage
{
    public Step2CompanyDetailsPage(Step2CompanyDetailsViewModel viewModel)
    {
        InitializeComponent();
        viewModel.DataForm = clientDetailsForm;
        BindingContext = viewModel;
    }
}