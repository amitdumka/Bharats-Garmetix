using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages;

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