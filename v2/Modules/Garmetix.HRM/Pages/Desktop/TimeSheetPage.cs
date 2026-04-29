using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Core.Models.HRM;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Desktop;

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