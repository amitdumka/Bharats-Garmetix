using Garmetix.Base.Views.Customs.Listing;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Desktop;

public class MonthlyAttendancePage : BaseListingPage
{
    private readonly MonthlyAttendancePageModel _viewModel;

     

    public MonthlyAttendancePage(MonthlyAttendancePageModel vm)
    {
        Title = "Monthly Attendance";
        //vm.AddUrl = nameof(EntryMonthlyAttendancePage);
        vm.AddUrl = "/main";
        vm.EnableAdd = false;
        BindingContext = _viewModel = vm;
    }
}
