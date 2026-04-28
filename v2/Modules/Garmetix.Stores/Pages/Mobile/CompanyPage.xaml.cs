using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.Base.Views.Customs.Listing;

namespace Garmetix.CoreBase.Stores.Pages.Mobile;

public partial class CompanyPage : BaseListViewPage
{
    private readonly CompanyPageModel _model;
    public CompanyPage(CompanyPageModel vm)
    {
        InitializeComponent();

        try
        {
            // _ = vm.HandleOnOnAppearing();
            InitializeComponent();
            Title = "Company";

            BindingContext = _model = vm;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            SentrySdk.CaptureMessage("Company Exp" + ex.Message);
        }
        finally
        {
        }
    }
}