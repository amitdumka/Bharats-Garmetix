using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Core.Models.HRM;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Mobile;

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