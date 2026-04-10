using Microsoft.Maui.Controls;

namespace Garmetix.AI.Billing.Views
{
    public partial class InvoiceEntryPage : ContentPage
    {
        public InvoiceEntryPage(ViewModels.InvoiceEntryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void MobileNoEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var vm = BindingContext as ViewModels.InvoiceEntryViewModel;
            if (vm != null && vm.SearchCustomerCommand.CanExecute(null))
            {
                vm.SearchCustomerCommand.Execute(null);
            }
        }
        // --- NEW: Wires the Search Box to the ViewModel Cache ---
        //private void ProductSearch_TextChanged(object sender, Syncfusion.Maui.Inputs.TextChangedEventArgs e)
        //{
        //    if (BindingContext is ViewModels.InvoiceEntryViewModel vm)
        //    {
        //        // Passes the typed text (Barcode or Name) into the high-speed search engine
        //        vm.UpdateFilteredProducts(e.NewTextValue);
        //    }
        //}
    }
}