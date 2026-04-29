using Garmetix.Base.Views.Customs.Listing;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Mobile;

public partial class MonthlyAttendancePage : BaseListViewPage
{
    private readonly MonthlyAttendancePageModel _viewModel;
     
    public MonthlyAttendancePage(MonthlyAttendancePageModel vm)
    {
        InitializeComponent();
        Title = "Monthly Attendance";
        //vm.AddUrl = nameof(EntryMonthlyAttendancePage);
        vm.AddUrl = "/main";
        vm.EnableAdd = false;
        vm.ListView = true;
        BindingContext = _viewModel = vm;
    }
}