using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Billing.PageModels.Invoices;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;

namespace Garmetix.Billing.PageModels
{
    public partial class InvoiceEntryPageModel : BaseInvoiceFormModel
    {
        public InvoiceEntryPageModel(InvoiceService invoiceService) : base(invoiceService)
        {
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

        // --- DATABASE SAVE ENGINE and FINAL ACTION COMMANDS ---
        [RelayCommand]
        public async Task SaveAndWhatsAppAsync()
        {
            if (await _invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: false, sendOverMsg: true))
            {
                await ClearFormAsync();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintA5Async()
        {
            if (await _invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: true, thermal: false, sendOverMsg: false))
            {
                await ClearFormAsync();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintThermalAsync()
        {
            if (await _invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: true, thermal: true, sendOverMsg: false))
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
            GlobalDiscountTypeInput = "Amount";
            PaymentModeInput = PaymentMode.Cash;

            PaymentAmountInput = 0; SelectedInvoiceItem = null;
            IsNewCustomer = false;
            SelectedProduct = null;
            CurrentInvoice = new InvoiceDTO();
            OnPropertyChanged(nameof(CurrentInvoice));
            await Task.CompletedTask;
        }

       
    }
}