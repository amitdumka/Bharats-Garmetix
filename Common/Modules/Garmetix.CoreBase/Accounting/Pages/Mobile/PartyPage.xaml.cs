using Garmetix.Core.Views.Customs.Listing;

namespace Garmetix.CoreBase.Accounting.Pages.Mobile;

public partial class PartyPage : BaseListViewPage
{
    private readonly PartyPageModel _viewModel;
    //protected override async void OnAppearing()
    //{
    //    //base.OnAppearing();
    //    //TODO: await _viewModel.HandleOnOnAppearing();
    //}
    public PartyPage(PartyPageModel viewModel)
    {
        try
        {
            InitializeComponent();
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            viewModel.AddUrl = $"Entry{Title}Page";
            viewModel.ListView = true;
            BindingContext = _viewModel = viewModel;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            SentrySdk.CaptureMessage("BankPage Exp" + ex.Message);
        }

    }
}