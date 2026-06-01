using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Pdf.Graphics;
using System.Collections.ObjectModel;
namespace Garmetix.Billing.PageModels
{

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


        private async Task GenerateAndPrintCreditNote(Invoice returnInvoice)
        {
            // 1. You already processed the return and have the 'returnReceipt' and 'customer' objects
            // Invoice returnReceipt = await _invoiceService.ProcessSaleReturnAsync(...);

            // 2. Generate the QR Code (Using the method we built earlier!)
            // We encode the ReturnInvoice ID so it can be scanned easily later
            byte[] qrBytes = InvoiceService.GenerateQRBarcodeCode(returnInvoice.InvoiceNumber, InvoiceCodeType.QRCode);

            // 3. Map the data to our DTO
            var creditNoteData = new CreditNoteDto
            {
                CustomerName = returnInvoice.CustomerName!,
                MobileNo = returnInvoice.CustomerMobileNumber,
                ReturnInvoiceNo = returnInvoice.InvoiceNumber,
                ReturnInvoiceDate =OriginalInvoice.OnDate, // The date they originally bought it
                NoteDate = returnInvoice.OnDate,            // The date of the return
                TotalAmount = returnInvoice.BillAmount,
                QrCodeImage = qrBytes
            };


            try
            {
                // 4. Generate the PDF A4 page
                string filePath = CreditNotePdfBuilder.GenerateCreditNote(creditNoteData);

                // 5. Trigger the Native Device Print/Share dialog
                //string fileName = $"CreditNote_{returnInvoice.InvoiceNumber}.pdf";
               // await PdfPrintService.SaveAndSharePdfAsync(pdfBytes, fileName);

                await Launcher.Default.OpenAsync(new OpenFileRequest { Title = "Print Credit Note", File = new ReadOnlyFile(filePath) });

            }
            catch (Exception pdfEx)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Print Error", $"Return saved, but failed to generate PDF: {pdfEx.Message}", "OK");
            }
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