using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Billing.Services;
using Garmetix.Core.Models.Inventory;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.PageModels.Invoices
{
    [QueryProperty(nameof(InvoiceId), "InvoiceId")]
    public partial class InvoiceEditPageModel : BaseInvoiceFormModel
    {
        [ObservableProperty] private string invoiceId = string.Empty;
        [ObservableProperty] private Invoice orginalInvoice;
        public ObservableCollection<EntryItem> EditItems { get; set; } = new();

        public InvoiceEditPageModel(InvoiceService invoiceService) : base(invoiceService)
        {
        }

        partial void OnInvoiceIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) _ = LoadInvoiceDataAsync(Guid.Parse(value));
        }

        private async Task LoadInvoiceDataAsync(Guid id)
        {
            IsBusy = true;
            try
            {
                OrginalInvoice = await _invoiceService.GetInvoiceByIdAsync(id);
                CurrentInvoice = OrginalInvoice.ToInvoiceDto();

                if (CurrentInvoice.GlobalDiscountAmount > 0)
                {
                    GlobalDiscountInput = CurrentInvoice.GlobalDiscountAmount;
                    GlobalDiscountTypeInput = "Amount";
                }

                EditItems.Clear();
                foreach (var item in OrginalInvoice.InvoiceItems)
                {
                    var dtoItem = item.ToEntryItem() ?? new EntryItem();
                    dtoItem.PropertyChanged += (s, e) => CalculateTotals();
                    EditItems.Add(dtoItem);
                }

                Payments.Clear();
                // Map original payments to the observable collection...

                CalculateTotals();
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Load Error", ex); }
            finally { IsBusy = false; }
        }
        // Replace 'partial void OnSelectedProductChanged' with this:
        protected override void HandleProductSelected(Product? value)
        {
            if (value != null)
            {
                var newItem = new EntryItem
                {
                    InvoiceId = CurrentInvoice.Id,
                    ProductName = value.Name,
                    Category = value.ProductType,
                    BasePrice = value.BasicPrice,
                    BilledQuantity = 1m
                };
                newItem.PropertyChanged += (s, e) => CalculateTotals();
                EditItems.Add(newItem);

                SearchText = string.Empty;
                CalculateTotals();
            }
        }
        

        public override void CalculateTotals()
        {
            // Similar logic to Entry Page, but iterating over EditItems instead of InvoiceItems
            SubTotal = EditItems.Sum(i => i.BasePrice * i.BilledQuantity);
            TotalDiscount = EditItems.Sum(i => i.DiscountAmount);
            TotalTax = EditItems.Sum(i => i.TaxAmount);

            // ... (Math logic remains the same)
        }

        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (EditItems.Count == 0) return;
            IsSaving = true;
            try
            {
                // Call your update logic here mapping EditItems and Payments to Entities
                // var result = await UpdateInvoicesAsync(CurrentInvoice, EditItems, Payments);
                await Shell.Current.GoToAsync("..");
            }
            finally { IsSaving = false; }
        }

        public override async Task ClearFormAsync()
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Cancel", "Discard your edits?", "Yes", "No");
            if (confirm) await Shell.Current.GoToAsync("..");
        }
    }
}