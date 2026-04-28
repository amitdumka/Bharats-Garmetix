using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages;

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
