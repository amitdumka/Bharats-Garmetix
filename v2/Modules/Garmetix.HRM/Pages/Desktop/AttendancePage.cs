using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages;

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
