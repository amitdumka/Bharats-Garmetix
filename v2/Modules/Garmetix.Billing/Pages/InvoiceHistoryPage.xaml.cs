using Garmetix.Billing.PageModels;
using Garmetix.Billing.Services;

namespace Garmetix.Billing.Pages
{
    public partial class InvoiceHistoryPage : ContentPage
    {
        private readonly InvoicesPageModel _viewModel;

        public InvoiceHistoryPage(InvoiceService invoiceService)
        {
            InitializeComponent();
            _viewModel = new InvoicesPageModel(invoiceService);
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