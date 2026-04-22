using Microsoft.Maui.Controls;

namespace Garmetix.AI.Billing.Views
{
    public partial class VoucherEntryPage : ContentPage
    {
        public VoucherEntryPage(ViewModels.VoucherEntryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // Catch the native SelectionChanged event
        private void OnPaymentModeSelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            // Safely cast the BindingContext to your ViewModel
            if (BindingContext is ViewModels.VoucherEntryViewModel vm)
            {
                // Trigger the logic to show/hide the Bank Account UI
                vm.CheckBankMode();
            }
        }
    }
}