using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.Pages.Mobile
{
    public partial class TimeSheetPage : BaseListViewPage
    {
        private readonly TimeSheetPageModel _viewModel;

         

        public TimeSheetPage(TimeSheetPageModel vm)
        {
            InitializeComponent();
            Title = "Time Sheet";
            vm.AddUrl = $"Entry{nameof(TimeSheet)}Page";
            vm.ListView = true;
            BindingContext = _viewModel = vm;
        }
    }
}