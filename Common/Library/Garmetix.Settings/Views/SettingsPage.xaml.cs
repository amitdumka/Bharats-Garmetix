using Microsoft.Maui.Controls;

namespace Garmetix.Settings.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly ViewModels.SettingsViewModel _viewModel;

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reload settings securely from the device every time the page opens
            await _viewModel.LoadSettingsAsync();
        }
    }
}