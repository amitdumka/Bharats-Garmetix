using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Core.Models.HRM;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Desktop;

public class SalaryStructurePage : BaseListingPage
{
    private readonly SalaryStructurePageModel _viewModel;

  

    public SalaryStructurePage(SalaryStructurePageModel vm)
    {
        Title = "Salary Structure";
        vm.AddUrl = $"Entry{nameof(SalaryStructure)}Page";
        BindingContext = _viewModel = vm;
    }
}