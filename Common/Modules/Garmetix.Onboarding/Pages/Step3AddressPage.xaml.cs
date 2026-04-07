using Garmetix.Onboarding.ViewModels;


namespace Garmetix.Onboarding.Pages;

public partial class Step3AddressPage : ContentPage
{
    public Step3AddressPage(Step3AddressViewModel step3AddressViewModel)
    {
        InitializeComponent();
        BindingContext = step3AddressViewModel;
    }
}