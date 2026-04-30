using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Core.Models.HRM;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Mobile
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