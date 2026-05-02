using Microsoft.Maui.Controls;
using AadwikaBilling.ViewModels;

namespace AadwikaBilling.Views
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