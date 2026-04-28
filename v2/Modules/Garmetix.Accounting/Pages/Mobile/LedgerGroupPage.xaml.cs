

namespace Garmetix.Accounting.Pages.Mobile;

public partial class LedgerGroupPage : BaseListViewPage
{
    private readonly LedgerGroupPageModel _viewModel;
    //protected override async void OnAppearing()
    //{
    //    //base.OnAppearing();
    //    //TODO: await _viewModel.HandleOnOnAppearing();
    //}
    public LedgerGroupPage(LedgerGroupPageModel vm)
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