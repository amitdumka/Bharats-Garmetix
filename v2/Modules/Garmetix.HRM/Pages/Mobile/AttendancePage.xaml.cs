using Garmetix.Core.Models.HRM; 
using Garmetix.Base.Views.Customs.Listing;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Mobile;

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