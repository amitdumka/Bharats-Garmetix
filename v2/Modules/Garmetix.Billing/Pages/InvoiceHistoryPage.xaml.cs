using Garmetix.AI.Billing.ViewModels;
using Garmetix.Billing.PageModels;
using Microsoft.Maui.Controls;

namespace Garmetix.AI.Billing.Views
{
    public partial class InvoiceHistoryPage : ContentPage
    {
        private readonly InvoicesPageModel _viewModel;

        public InvoiceHistoryPage()
        {
            InitializeComponent();
            _viewModel = new InvoicesPageModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reloads data and refreshes filters whenever the user opens this page
            await _viewModel.LoadDataAsync();
        }
    }
}