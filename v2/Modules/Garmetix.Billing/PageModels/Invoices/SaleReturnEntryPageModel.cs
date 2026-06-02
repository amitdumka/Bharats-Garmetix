using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Services;
using Garmetix.Core.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.PageModels.Invoices
{
    public partial class SaleReturnEntryPageModel : BaseInvoiceFormModel
    {
        [ObservableProperty] private string _searchInvoiceNo;
        [ObservableProperty] private Invoice _originalInvoice;

        public ObservableCollection<ReturnItemWrapper> ReturnItems { get; } = new();

        public SaleReturnEntryPageModel(InvoiceService invoiceService) : base(invoiceService)
        {
        }

        [RelayCommand]
        public async Task SearchInvoiceAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchInvoiceNo)) return;

            var invoice = await BaseInvoiceFormModel.GetContext().Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceNumber == SearchInvoiceNo);

            if (invoice == null) return;

            OriginalInvoice = invoice;
            ReturnItems.Clear();

            foreach (var item in invoice.InvoiceItems)
            {
                var wrapper = new ReturnItemWrapper
                {
                    ProductId = item.ProductId,
                    ProductName = item.Barcode,
                    Barcode = item.Barcode,
                    Rate = item.BasePrice,
                    OriginalQty = item.BilledQuantity,
                    ReturnQuantity = 0,
                    IsSelected = false
                };

                wrapper.PropertyChanged += (s, e) => CalculateTotals();
                ReturnItems.Add(wrapper);
            }
        }

        public override void CalculateTotals()
        {
            GrandTotal = ReturnItems.Where(x => x.IsSelected).Sum(x => x.TotalRefund);
            // Sync to DTO if necessary for returns
        }

        [RelayCommand]
        public async Task ProcessReturnAsync()
        {
            var itemsToReturn = ReturnItems
                .Where(x => x.IsSelected && x.ReturnQuantity > 0)
                .Select(x => new InvoiceItem
                {
                    ProductId = x.ProductId,
                    Barcode = x.Barcode,
                    BilledQuantity = x.ReturnQuantity,
                    BasePrice = x.Rate,
                }).ToList();

            if (!itemsToReturn.Any()) return;

            try
            {
                await _invoiceService.ProcessSaleReturnAsync(OriginalInvoice.Id, itemsToReturn, PaymentModeInput);
                await ClearFormAsync();
            }
            catch (Exception ex) { /* Handle */ }
        }

        public override Task ClearFormAsync()
        {
            OriginalInvoice = null;
            ReturnItems.Clear();
            SearchInvoiceNo = string.Empty;
            GrandTotal = 0;
            return Task.CompletedTask;
        }
    }
}