
using Garmetix.Core.Models.HRM;
using Garmetix.HRM.PageModels;
using Garmetix.Base.Views.Customs.Listing;

namespace Garmetix.HRM.Pages.Desktop;

public class SalaryPaymentPage : BaseListingPage
{
    private readonly SalaryPaymentPageModel _viewModel;

    

    public SalaryPaymentPage(SalaryPaymentPageModel vm)
    {
        Title = "Salary";
        vm.AddUrl = $"Entry{nameof(SalaryPayment)}Page";
        BindingContext = _viewModel = vm;
    }
}
