using Garmetix.Onboarding.ViewModels;

namespace Garmetix.Onboarding.Pages;

public partial class Step5KeyPersonalDetailsPage : ContentPage
{
    public Step5KeyPersonalDetailsPage(Step5KeyPersonalInfoViewModel viewModel)
    {
        viewModel.DataForm = clientDetailsForm;
        InitializeComponent();
        BindingContext = viewModel;

    }
}