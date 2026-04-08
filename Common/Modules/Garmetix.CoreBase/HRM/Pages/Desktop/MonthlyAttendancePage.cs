using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;

namespace Garmetix.CoreBase.HRM.Pages;

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
