using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Core.Models.HRM;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Desktop;

public class SalaryPaySlipPage : BaseListingPage
{
    private readonly SalaryPaySlipPageModel _viewModel;

    

    public SalaryPaySlipPage(SalaryPaySlipPageModel vm)
    {
        Title = "Salary Slip";
        vm.AddUrl = $"Entry{nameof(SalaryPaySlip)}Page";
        BindingContext = _viewModel = vm;
    }
}
