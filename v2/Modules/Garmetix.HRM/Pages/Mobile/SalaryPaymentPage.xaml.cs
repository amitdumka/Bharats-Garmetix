using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages.Mobile;

public partial class SalaryPaymentPage : BaseListViewPage
{
    private readonly SalaryPaymentPageModel _viewModel;
     
    public SalaryPaymentPage(SalaryPaymentPageModel vm)
    {
        InitializeComponent();
        Title = "Salary";
        vm.AddUrl = $"Entry{nameof(SalaryPayment)}Page";
        vm.ListView = true;
        BindingContext = _viewModel = vm;
    }
}