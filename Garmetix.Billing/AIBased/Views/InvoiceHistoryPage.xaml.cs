using Garmetix.Billing.AIBased.ViewModels;
using Microsoft.Maui.Controls;

namespace Garmetix.AI.Billing.Views
{
    public partial class InvoiceHistoryPage : ContentPage
    {
        private readonly InvoiceHistoryViewModel _viewModel;

        public InvoiceHistoryPage()
        {
            InitializeComponent();
            _viewModel = new InvoiceHistoryViewModel();
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