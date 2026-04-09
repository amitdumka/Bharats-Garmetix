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
    }
}