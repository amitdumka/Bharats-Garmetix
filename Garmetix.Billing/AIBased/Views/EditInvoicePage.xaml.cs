using Microsoft.Maui.Controls;

namespace Garmetix.AI.Billing.Views
{
    public partial class EditInvoicePage : ContentPage
    {
        public EditInvoicePage(ViewModels.EditInvoiceViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}