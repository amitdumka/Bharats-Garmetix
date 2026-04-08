using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages;

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
