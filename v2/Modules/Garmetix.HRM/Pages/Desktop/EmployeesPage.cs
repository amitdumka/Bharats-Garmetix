using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Core.Models.HRM;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Desktop;

public class EmployeesPage : BaseListingPage
{
    private readonly EmployeePageModel _viewModel;

     

    public EmployeesPage(EmployeePageModel vm)
    {
        Title = "Employees";
        vm.AddUrl = $"Entry{nameof(Employee)}Page";
        BindingContext = _viewModel = vm;
    }
}
