using Garmetix.AI.Billing.ViewModels;

namespace  Garmetix.AI.Billing.Views
{
    public partial class InvoiceEntryPage : ContentPage
    {
        public InvoiceEntryPage(ViewModels.InvoiceEntryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // Add this method to handle the Unfocused event
        private void MobileNoEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var vm = BindingContext as ViewModels.InvoiceEntryViewModel;
            if (vm != null && vm.SearchCustomerCommand.CanExecute(null))
            {
                vm.SearchCustomerCommand.Execute(null);
            }
        }
    }
}