using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages;

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
