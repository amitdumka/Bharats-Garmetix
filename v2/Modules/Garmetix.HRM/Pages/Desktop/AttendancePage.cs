using Garmetix.Core.Models.HRM;
using Garmetix.Base.Views.Customs.Listing;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Desktop;

public class AttendancePage : BaseListingPage
{
    private readonly AttendancePageModel _viewModel;

    

    public AttendancePage(AttendancePageModel vm)
    {
        Title = "Attendance";
        vm.AddUrl = $"Entry{nameof(Attendance)}Page";
        BindingContext = _viewModel = vm;

    }
}
