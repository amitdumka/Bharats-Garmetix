using Garmetix.AI.Billing.ViewModels;

namespace  Garmetix.AI.Billing.Views
{
    public partial class InvoiceEntryPage : ContentPage
    {
        public InvoiceEntryPage(InvoiceEntryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}