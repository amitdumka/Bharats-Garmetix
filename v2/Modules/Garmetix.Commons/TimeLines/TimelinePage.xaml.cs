namespace Garmetix.CoreBase.TimeLines;

public partial class TimelinePage : ContentPage
{
	 
    private readonly TimelinePageModel _viewModel;

    public TimelinePage(TimelinePageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel; // Set the ViewModel as the BindingContext
    }

    // Load data when the page appears
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTimelineCommand.ExecuteAsync(null);
    }
}