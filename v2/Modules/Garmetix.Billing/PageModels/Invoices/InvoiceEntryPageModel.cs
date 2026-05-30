using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Garmetix.Billing.Models;
using Garmetix.Billing.PageModels.Invoices;
using Garmetix.Billing.Services;
using Garmetix.Core.Models.Inventory;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Garmetix.Billing.PageModels.Invoices
{
    public partial class InvoiceEntryPageModel : BaseInvoiceFormModel
    {
        public ObservableCollection<EntryItem> InvoiceItems { get; set; } = new();
        [ObservableProperty] protected EntryItem? selectedInvoiceItem;

       
        public InvoiceEntryPageModel(InvoiceService invoiceService) : base(invoiceService)
        {
        }

        protected override void HandleProductSelected(Product? value)
        {
            if (value != null)
            {
                AddProductToInvoice(value);
                SearchText = string.Empty;
            }
        }
        private void AddProductToInvoice(Product product)
        {
            try
            {
                var newItem = new EntryItem
                {
                    Barcode = product.Barcode,
                    Category = product.ProductType,
                    BasePrice = product.BasicPrice,
                    ProductName = product.Name,
                    ProductId = product.Id,
                    MRP = product.MRP,
                    Unit = product.Unit,
                    BilledQuantity = 1m,
                    DiscountPercentage = 0
                };

                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
                CalculateTotals();
            }
            catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Add Product Error", ex); }
        }

        [RelayCommand]
        public void RemoveInvoiceItem(EntryItem item)
        {
            if (item == null || !InvoiceItems.Contains(item)) return;
            item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Remove(item);
            CalculateTotals();
        }

        protected void InvoiceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(EntryItem.BasePrice) or nameof(EntryItem.BilledQuantity) or nameof(EntryItem.DiscountAmount))
            {
                CalculateTotals();
            }
        }

        public override void CalculateTotals()
        {
            try
            {
                SubTotal = InvoiceItems.Sum(i => i.BasePrice * i.BilledQuantity);
                TotalDiscount = InvoiceItems.Sum(i => i.DiscountAmount);
                TotalTax = InvoiceItems.Sum(i => i.TaxAmount);

                decimal preDiscountTotal = (SubTotal - TotalDiscount) + TotalTax;
                decimal globalDiscountCalculated = GlobalDiscountTypeInput == "%" ? preDiscountTotal * (GlobalDiscountInput / 100m) : GlobalDiscountInput;

                decimal rawGrandTotal = preDiscountTotal - globalDiscountCalculated;
                if (rawGrandTotal < 0) rawGrandTotal = 0;

                GrandTotal = Math.Round(rawGrandTotal, 0, MidpointRounding.AwayFromZero);
                RoundOffAmount = GrandTotal - rawGrandTotal;
                PaidAmount = Payments.Sum(p => p.Amount);
                BalanceAmount = GrandTotal - PaidAmount;

                // Sync with DTO
                CurrentInvoice.SubTotal = SubTotal;
                CurrentInvoice.TotalTax = TotalTax;
                CurrentInvoice.GlobalDiscountAmount = globalDiscountCalculated;
                CurrentInvoice.RoundOffAmount = RoundOffAmount;
                CurrentInvoice.GrandTotal = GrandTotal;
                CurrentInvoice.PaidAmount = PaidAmount;
            }
            catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Calculation Error", ex); }
        }

        [RelayCommand]
        public async Task SaveAndPrintA5Async()
        {
            if (await _invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: true, thermal: false, sendOverMsg: false))
            {
                await ClearFormAsync();
            }
        }

        public override async Task ClearFormAsync()
        {
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear();
            Payments.Clear();
            GlobalDiscountInput = 0;
            PaymentAmountInput = 0;
            IsNewCustomer = false;
            SelectedProduct = null;
            CurrentInvoice = new InvoiceDTO();
            await Task.CompletedTask;
        }
    }
}