using Garmetix.Onboarding.ViewModels;


namespace Garmetix.Onboarding.Pages;

public partial class ReviewPage : ContentPage
{
    public ReviewPage(ReviewViewModel reviewViewModel)
    {

        InitializeComponent();
        BindingContext = reviewViewModel;
    }
    // Optional: Refresh data if user navigates back to this page after editing
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReviewViewModel vm)
        {
            vm.DataToReview = vm.DataToReview; // This will trigger property changed if implemented correctly in BaseViewModel or ObservableObject
        }
    }
}