using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages.Mobile;

public partial class AttendancePage : BaseListViewPage
{
    private readonly AttendancePageModel _viewModel;
    
    public AttendancePage(AttendancePageModel vm)
    {
        InitializeComponent();
        Title = "Attendance";
        vm.AddUrl = $"Entry{nameof(Attendance)}Page";
        vm.ListView = true;
        BindingContext = _viewModel = vm;
    }
}