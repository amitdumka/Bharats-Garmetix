using Garmetix.Onboarding.ViewModels;

namespace Garmetix.Onboarding.Pages;

public partial class CompletionPage : ContentPage
{
    public CompletionPage(CompletionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}