using Microsoft.Maui.Controls;

namespace Garmetix.Commons.Dashboard.Pages.Dashboards
{
    public partial class DashboardPage : ContentPage
    {
        private readonly ViewModels.DashboardViewModel _viewModel;

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reloads the charts and metrics every time you navigate back to the home screen
            await _viewModel.LoadDashboardAsync();
        }
    }
}