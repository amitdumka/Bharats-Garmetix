using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Services;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.PageModels.Invoices
{
    [QueryProperty(nameof(ReturnInvoiceId), "ReturnInvoiceId")]
    public partial class SaleReturnEditPageModel : BaseInvoiceFormModel
    {
        [ObservableProperty] private string returnInvoiceId = string.Empty;

        // Similar to Entry, but representing already returned items
        public ObservableCollection<ReturnItemWrapper> EditReturnItems { get; } = new();

        public SaleReturnEditPageModel(InvoiceService invoiceService) : base(invoiceService)
        {
        }

        partial void OnReturnInvoiceIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) _ = LoadReturnDataAsync(Guid.Parse(value));
        }

        private async Task LoadReturnDataAsync(Guid id)
        {
            IsBusy = true;
            try
            {
                // 1. Load the existing Return Invoice from the database
                // 2. Map existing return items into EditReturnItems collection
                // 3. Attach PropertyChanged events to recalculate totals on edit
                CalculateTotals();
            }
            finally { IsBusy = false; }
        }

        public override void CalculateTotals()
        {
            GrandTotal = EditReturnItems.Where(x => x.IsSelected).Sum(x => x.TotalRefund);
        }

        [RelayCommand]
        public async Task SaveReturnEditsAsync()
        {
            // Validate and save updated return limits/refunds back to DB
        }

        public override async Task ClearFormAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}