using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Core.Models.Stores;

namespace Garmetix.CoreBase.Stores.Pages.Mobile;

public partial class StorePage : BaseListViewPage
{
    private readonly StorePageModel _viewModel;

    

    public StorePage(StorePageModel vm)
    {
        try
        {
            // _= vm.HandleOnOnAppearing();
            InitializeComponent();
            Title = "Stores";
            vm.AddUrl = $"Entry{nameof(Store)}Page";
            BindingContext = _viewModel = vm;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            SentrySdk.CaptureMessage("StorePPage Exp" + ex.Message);
        }
        finally
        {
        }
    }
}