using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages;

public class TimeSheetPage : BaseListingPage
{
    private readonly TimeSheetPageModel _viewModel;

    

    public TimeSheetPage(TimeSheetPageModel vm)
    {
        Title = "Time Sheet";
        vm.AddUrl = $"Entry{nameof(TimeSheet)}Page";
        BindingContext = _viewModel = vm;
    }
}
