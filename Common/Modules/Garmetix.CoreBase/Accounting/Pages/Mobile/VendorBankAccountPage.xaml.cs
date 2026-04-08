using Garmetix.Core.Views.Customs.Listing;

namespace Garmetix.CoreBase.Accounting.Pages.Mobile;

public partial class VendorBankAccountPage : BaseListViewPage
{
    private readonly VendorBankAccountPageModel _pageModel;
    //protected override async void OnAppearing()
    //{
    //    //base.OnAppearing();
    //    //await _pageModel.HandleOnOnAppearing();
    //}
    public VendorBankAccountPage(VendorBankAccountPageModel pageModel)
    {
        try
        {
            InitializeComponent();
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            pageModel.AddUrl = $"Entry{Title}Page";
            pageModel.ListView = true;
            BindingContext = _pageModel = pageModel;

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