using Garmetix.Billing.PageModels;
using Microsoft.Maui.Controls;

namespace Garmetix.Billing.Pages
{
    public partial class EditInvoicePage : ContentPage
    {
        public EditInvoicePage(InvoiceEditPageModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}