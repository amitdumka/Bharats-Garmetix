using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
namespace Garmetix.Billing.PageModels
{
    // UI-Specific wrapper to handle selection and return quantity tracking
    public partial class ReturnItemWrapper : ObservableObject
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public decimal Rate { get; set; }
        public decimal OriginalQty { get; set; }

        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal LineTotal { get; set; } = 0;

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private decimal _returnQuantity;

        [ObservableProperty]
        private decimal _totalRefund;

        // Auto-calculate totals and prevent returning more than purchased
        partial void OnReturnQuantityChanged(decimal value)
        {
            if (value > OriginalQty)
            {
                ReturnQuantity = OriginalQty; // Cap at max purchased
            }
            TotalRefund = ReturnQuantity * Rate;
        }
    }

    public partial class SaleReturnViewModel : ObservableObject
    {
        private readonly InvoiceService _invoiceService;

        [ObservableProperty]
        private string _searchInvoiceNo;

        [ObservableProperty]
        private Invoice _originalInvoice;

        [ObservableProperty]
        private decimal _grandTotalRefund;

        [ObservableProperty]
        private PaymentMode _selectedRefundMode = PaymentMode.Cash;

        public string[] RefundModes { get; } = new[] { "Cash", "UPI", "Card", "Store Credit" };

        public ObservableCollection<ReturnItemWrapper> ReturnItems { get; } = new();

        public SaleReturnViewModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [RelayCommand]
        public async Task SearchInvoiceAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchInvoiceNo))
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Please enter or scan an Invoice Number.", "OK");
                return;
            }

            // Fetch the invoice (Ensure you have a method to get by InvoiceNo in your DB context)
            var context = _invoiceService.GetContext();
            var invoice = await context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceNumber== SearchInvoiceNo);

            if (invoice == null)
            {
                await Application.Current.MainPage.DisplayAlert("Not Found", "Invoice not found.", "OK");
                return;
            }

            OriginalInvoice = invoice;
            ReturnItems.Clear();

            // Populate the UI wrapper collection
            foreach (var item in invoice.InvoiceItems)
            {
                var wrapper = new ReturnItemWrapper
                {
                    ProductId = item.ProductId,
                    ProductName = item.Barcode,
                    Barcode = item.Barcode,
                    Rate = item.BasePrice,
                    OriginalQty = item.BilledQuantity,
                    DiscountAmount=item.DiscountAmount,
                    TaxAmount=item.TaxAmount,
                   LineTotal= item.LineTotal,
                    ReturnQuantity = 0, // Default to 0 until user types a number
                    IsSelected = false
                };

                // Recalculate the Grand Total whenever a row changes
                wrapper.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ReturnItemWrapper.IsSelected) ||
                        e.PropertyName == nameof(ReturnItemWrapper.TotalRefund))
                    {
                        CalculateTotalRefund();
                    }
                };

                ReturnItems.Add(wrapper);
            }
        }

        private void CalculateTotalRefund()
        {
            GrandTotalRefund = ReturnItems
                .Where(x => x.IsSelected)
                .Sum(x => x.TotalRefund);
        }

        [RelayCommand]
        public async Task ProcessReturnAsync()
        {
            // Extract only the selected items with a valid quantity
            var itemsToReturn = ReturnItems
                .Where(x => x.IsSelected && x.ReturnQuantity > 0)
                .Select(x => new InvoiceItem
                {
                    ProductId = x.ProductId,
                   // ProductName = x.ProductName,
                    Barcode = x.Barcode,
                    BilledQuantity = x.ReturnQuantity,
                    BasePrice = x.Rate, 
                    DiscountAmount = x.DiscountAmount,
                    TaxAmount = x.TaxAmount,
                    
                }).ToList();

            if (!itemsToReturn.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Select at least one item with a valid return quantity.", "OK");
                return;
            }

            try
            {
                // Call the transaction service we built earlier
                var returnReceipt = await _invoiceService.ProcessSaleReturnAsync(
                    OriginalInvoice.Id,
                    itemsToReturn,
                    SelectedRefundMode);

                await Application.Current.MainPage.DisplayAlert("Success", $"Return Processed Successfully.\nRefund Amount: ₹ {returnReceipt.BillAmount:N2}", "OK");

                // Clear UI for the next customer
                OriginalInvoice = null;
                ReturnItems.Clear();
                SearchInvoiceNo = string.Empty;
                GrandTotalRefund = 0;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Database Error", ex.Message, "OK");
            }
        }
    }
}