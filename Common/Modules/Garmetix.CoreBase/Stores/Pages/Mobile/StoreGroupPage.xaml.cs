using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.Core.Views.Customs.Listing;

namespace Garmetix.CoreBase.Stores.Pages.Mobile;

public partial class StoreGroupPage : BaseListViewPage
{
    private readonly StoreGroupPageModel _viewModel;

   

    public StoreGroupPage(StoreGroupPageModel vm)
    {
        try
        {
             
            InitializeComponent();
            Title = "Store Groups";

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