using Garmetix.Core.Views.Customs.Listing;

namespace Garmetix.CoreBase.Accounting.Pages.Mobile;

public partial class BankAccountPage : BaseListViewPage
{
    private readonly BankAccountPageModel _viewModel;
    //protected override async void OnAppearing()
    //{
    //    //base.OnAppearing();
    //    //TODO: await _viewModel.HandleOnOnAppearing();
    //}

    public BankAccountPage(BankAccountPageModel vm)
    {
        try
        {
            InitializeComponent();
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            vm.ListView = true;
            BindingContext = _viewModel = vm;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            SentrySdk.CaptureMessage($"{GetType().Name} Exp" + ex.Message);
        }
        finally
        {
        }
    }
}