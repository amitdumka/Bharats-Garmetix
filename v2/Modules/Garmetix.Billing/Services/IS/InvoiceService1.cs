using CommunityToolkit.Maui.Extensions;
using Garmetix.Billing.Pages.Popups;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using ZXing;
using ZXing.SkiaSharp;
namespace Garmetix.Billing.Services
{
    public partial class InvoiceService : BaseInvoiceService
    {

        //public async Task GenerateAndPrintCreditNote(Invoice returnInvoice)
        //{
        //    // 1. You already processed the return and have the 'returnReceipt' and 'customer' objects
        //    // Invoice returnReceipt = await _invoiceService.ProcessSaleReturnAsync(...);

        //    // 2. Generate the QR Code (Using the method we built earlier!)
        //    // We encode the ReturnInvoice ID so it can be scanned easily later
        //    byte[] qrBytes = GenerateInvoiceCode(returnInvoice, InvoiceCodeType.QRCode);

        //    // 3. Map the data to our DTO
        //    var creditNoteData = new CreditNoteDto
        //    {
        //        CustomerName = ActiveCustomer.Name,
        //        MobileNo = ActiveCustomer.MobileNo,
        //        ReturnInvoiceNo = returnInvoice.InvoiceNumber,
        //        ReturnInvoiceDate = returnInvoice.OnDate, // The date they originally bought it
        //        NoteDate = returnInvoice.OnDate,            // The date of the return
        //        TotalAmount = returnInvoice.BillAmount,
        //        QrCodeImage = qrBytes
        //    };


        //    try
        //    {
        //        // 4. Generate the PDF A4 page
        //        byte[] pdfBytes = CreditNotePdfBuilder.GenerateCreditNote(creditNoteData);

        //        // 5. Trigger the Native Device Print/Share dialog
        //        string fileName = $"CreditNote_{returnInvoice.InvoiceNumber}.pdf";
        //        await PdfPrintService.SaveAndSharePdfAsync(pdfBytes, fileName);
        //    }
        //    catch (Exception pdfEx)
        //    {
        //        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Print Error", $"Return saved, but failed to generate PDF: {pdfEx.Message}", "OK");
        //    }
        //}

        // Sale Return Code here 
        /// <summary>
        /// Processes a sales return, restores inventory, and generates a return invoice.
        /// </summary>
        public async Task<Invoice> ProcessSaleReturnAsync(Guid originalInvoiceId, List<InvoiceItem> returnedItems, PaymentMode refundPaymentMode)
        {
            var context = GetContext();

            // 1. Fetch the original invoice to validate
            var originalInvoice = await context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == originalInvoiceId);

            if (originalInvoice == null)
                throw new Exception("Original invoice not found.");

            if (returnedItems == null || !returnedItems.Any())
                throw new Exception("No items selected for return.");

            // 2. Create the new Return Invoice (Credit Note)
            var returnInvoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = $"RET-{originalInvoice.InvoiceNumber}-{DateTime.Now:HHmmss}",
                OnDate = DateTime.Now,
                ReturnInvoice = true,                   // Flagged as a return!
                OriginalInvoiceId = originalInvoice.Id,
                PaymentMode = refundPaymentMode,
                InvoiceItems = new List<InvoiceItem>()
            };

            decimal totalRefundAmount = 0;

            // 3. Begin Database Transaction safely
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                foreach (var returnItem in returnedItems)
                {
                    // Validate against original purchase
                    var originalItem = originalInvoice.InvoiceItems
                        .FirstOrDefault(x => x.ProductId == returnItem.ProductId);

                    if (originalItem == null)
                        throw new Exception($"Product {returnItem.Barcode} was not on the original invoice.");

                    // Optional: Check if already returned by querying past returns
                    // ... (Logic to prevent double-returns goes here) ...

                    decimal refundAmount = returnItem.BilledQuantity * returnItem.BasePrice;
                    totalRefundAmount += refundAmount;

                    // Add item to the return invoice
                    //TODO: handle for Store and Compamy id and proper init of obkect

                    returnInvoice.InvoiceItems.Add(returnItem);

                    //returnInvoice.InvoiceItems.Add(new InvoiceItem
                    //{
                    //    Id = Guid.NewGuid(),
                    //    InvoiceId = returnInvoice.Id,
                    //    ProductId = returnItem.ProductId,
                    //    Barcode = returnItem.ProductName,
                    //    BilledQuantity = returnItem.ReturnQuantity, // Keep positive, the IsSaleReturn flag dictates the math
                    //    BasePrice = returnItem.Rate,
                    //    Amount = refundAmount
                    //});

                    // 4. Restore Inventory (Add the stock back to the shelf)
                    var stock = await context.Stocks.FirstOrDefaultAsync(s => s.ProductId == returnItem.ProductId);
                    if (stock != null)
                    {
                        stock.SoldQty += returnItem.BilledQuantity;
                        context.Stocks.Update(stock);
                    }
                }

                returnInvoice.NetAmount = totalRefundAmount;
                returnInvoice.BillAmount = totalRefundAmount; // Add tax logic here if necessary

                // 5. Save to Database
                await context.Invoices.AddAsync(returnInvoice);
                await context.SaveChangesAsync();

                // 6. Commit Transaction
                await transaction.CommitAsync();

                return returnInvoice;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                System.Diagnostics.Debug.WriteLine($"Return failed: {ex.Message}");
                throw; // Rethrow to let the UI handle the error message
            }
        }
    }

    //Imlementing Extra and Spl method to handle invoicing
    public partial class InvoiceService : BaseInvoiceService
    {
        /// <summary>
        /// Clear the last saved invoice data from memory. This is useful when we want to clear the cache after printing or when we want to save a new invoice without using the last saved data.
        /// </summary>
        public void ClearLastSavedData()
        {
            _lastSavedInvoice = null;
            _lastSaveditems = null;
            _lastSavedPayments = null;
            _lastSavedCardPayments = null;
            _isSaved = false;
        }
        public async Task<Guid?> GetInvoiceIdByNumberAsync(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                return null;
            var invoice = await GetContext().Invoices
                .Where(i => i.InvoiceNumber == invoiceNumber)
                .Select(i => new { i.Id })
                .FirstOrDefaultAsync();
            return invoice?.Id;
        }

        // This can be move to view Model and Result can be feed fileter fileds.
        public async Task OpenScanDialogAsync()
        {
            // 1. Show the Popup and wait for the user to scan and click a button
            var popup = new InvoiceScanPopup();
            var result1 = await Shell.Current.CurrentPage.ShowPopupAsync<ScanPopupResult>(popup);// as ScanPopupResult;

            // 2. If they clicked cancel, do nothing
            if (result1 == null) return;
            var result = result1.Result as ScanPopupResult;
            if (result == null) return;
            // 3. Resolve the actual Database ID from the scanned string 
            // (You can add this helper method to your InvoiceService)
            Guid? invoiceId = await GetInvoiceIdByNumberAsync(result.ScannedCode);

            if (invoiceId == null)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Invoice not found in database.", "OK");
                return;
            }

            // 4. Navigate based on the button they clicked in the popup
            try
            {
                switch (result.ActionRequested)
                {
                    case "Edit":
                        await Shell.Current.GoToAsync($"EditInvoicePage?InvoiceId={invoiceId}");
                        break;
                    case "View":
                        await Shell.Current.GoToAsync($"ViewInvoicePage?InvoiceId={invoiceId}");
                        break;
                    case "Return":
                        await Shell.Current.GoToAsync($"SaleReturnPage?InvoiceId={invoiceId}");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Routing Error: {ex.Message}");
            }
        }

        //
        /// <summary>
        /// Generates a visual QR Code or Barcode for an invoice.
        /// </summary>
        public static byte[] GenerateInvoiceCode(Invoice invoice, InvoiceCodeType codeType)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");

            string contentToEncode;
            BarcodeFormat format;
            var options = new ZXing.Common.EncodingOptions();

            // 1. Configure based on the requested Code Type
            if (codeType == InvoiceCodeType.QRCode)
            {
                if (invoice.Id == Guid.Empty)
                    throw new ArgumentException("Valid invoice ID is required for a QR Code.");

                contentToEncode = invoice.Id.ToString();
                format = BarcodeFormat.QR_CODE;
                options.Width = 250;
                options.Height = 250;
                options.Margin = 1;
            }
            else // Barcode1D
            {
                if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                    throw new ArgumentException("Invoice Number is required for a Barcode.");

                contentToEncode = invoice.InvoiceNumber;
                format = BarcodeFormat.CODE_128;
                options.Width = 400;
                options.Height = 100;
                options.Margin = 2;
                options.PureBarcode = false; // Prints the text below the barcode
            }

            // 2. Generate the image (Shared Logic)
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = format,
                    Options = options
                };

                using var bitmap = writer.Write(contentToEncode);
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return data.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{codeType} Generation Failed: {ex.Message}");
                return null; // Return null gracefully so the UI doesn't crash
            }
        }
        public static byte[] GenerateQRBarcodeCode(string contentToEncode, InvoiceCodeType codeType)
        {
            if (string.IsNullOrEmpty(contentToEncode))
                throw new ArgumentNullException(nameof(contentToEncode), "Content to encode cannot be null or empty .");

            // string contentToEncode;
            BarcodeFormat format;
            var options = new ZXing.Common.EncodingOptions();

            // 1. Configure based on the requested Code Type
            if (codeType == InvoiceCodeType.QRCode)
            {
                format = BarcodeFormat.QR_CODE;
                options.Width = 250;
                options.Height = 250;
                options.Margin = 1;
            }
            else // Barcode1D
            {

                // contentToEncode = invoice.InvoiceNumber;
                format = BarcodeFormat.CODE_128;
                options.Width = 400;
                options.Height = 100;
                options.Margin = 2;
                options.PureBarcode = false; // Prints the text below the barcode
            }

            // 2. Generate the image (Shared Logic)
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = format,
                    Options = options
                };

                using var bitmap = writer.Write(contentToEncode);
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return data.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{codeType} Generation Failed: {ex.Message}");
                return null; // Return null gracefully so the UI doesn't crash
            }
        }
        /// <summary>
        /// Generates a QR Code containing the unique Invoice ID.
        /// Ideal for robust, error-corrected scanning.
        /// </summary>
        public static byte[] CreateQRCodeForInvoice(Invoice invoice)
        {
            if (invoice == null || invoice.Id == Guid.Empty)
                throw new ArgumentNullException(nameof(invoice), "Valid invoice is required to generate a QR Code.");

            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 250,
                        Height = 250,
                        Margin = 1
                    }
                };

                // Encode the unique GUID into the QR code
                using var bitmap = writer.Write(invoice.Id.ToString());
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return data.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"QR Code Generation Failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Generates a standard 1D Barcode (Code128) containing the Invoice Number.
        /// Ideal for basic laser scanners at a retail checkout counter.
        /// </summary>
        public static byte[] CreateBarCodeForInvoice(Invoice invoice)
        {
            if (invoice == null || string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                throw new ArgumentNullException(nameof(invoice), "Invoice Number is required to generate a Barcode.");

            try
            {
                var writer = new BarcodeWriter
                {
                    // CODE_128 is the most robust 1D barcode format for alphanumeric strings
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 400,
                        Height = 100,
                        Margin = 2,
                        PureBarcode = false // Set to true if you don't want the text printed below the bars
                    }
                };

                // Encode the human-readable Invoice Number (e.g., "INV-2026-001")
                using var bitmap = writer.Write(invoice.InvoiceNumber);
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return data.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Barcode Generation Failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Processes a scanned string from a camera or hardware scanner.
        /// Automatically detects if it is a QR Code (Guid) or Barcode (String) and navigates.
        /// </summary>
        public async Task ProcessScannedCodeAsync(string scannedValue)
        {
            if (string.IsNullOrWhiteSpace(scannedValue))
                return;

            try
            {
                Guid targetInvoiceId;

                // 1. Is it a QR Code? (Check if the scanned string is a valid Guid)
                if (Guid.TryParse(scannedValue, out targetInvoiceId))
                {
                    // Success: We already have the exact ID.
                }
                // 2. Is it a Barcode? (It's a string like "INV-001")
                else
                {
                    // We need to look up the Invoice ID based on the Invoice Number
                    // NOTE: Replace `GetContext()` with your actual DbContext or Repository call
                    var context = GetContext();
                    var invoice = await context.Invoices
                        .FirstOrDefaultAsync(i => i.InvoiceNumber == scannedValue);

                    if (invoice == null)
                    {
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Not Found", $"No invoice found matching: {scannedValue}", "OK");
                        return;
                    }

                    targetInvoiceId = invoice.Id;
                }

                // 3. Navigate securely to the Edit Page
                // Ensure this is run on the Main UI Thread to prevent COMExceptions
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Shell.Current.GoToAsync($"EditInvoicePage?InvoiceId={targetInvoiceId}");
                    }
                    catch (Exception navEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Navigation Failed: {navEx.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Scanning Processing Failed: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Scan Error", "Failed to process the scanned code.", "OK");
            }
        }



        public async Task LoadInvoiceAsync(Guid? inv = null, string? invoiceNumber = null)
        {
            //Write the logic to load the invoice based on invoice number or invoice id. This can be used for return and exchange process to quickly fetch the invoice details.
            Invoice invoice;

            if (invoiceNumber != null)
            {

                invoice = (await GetContext().Invoices.Include(i => i.InvoiceItems).Include(i => i.Payments).Include(i => i.CardPayments).FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber)) ?? new Invoice { InvoiceNumber = "NOTFOUND" };
            }
            else if (inv != null)
            {

                invoice = (await GetContext().Invoices.Include(i => i.InvoiceItems).Include(i => i.Payments).Include(i => i.CardPayments).FirstOrDefaultAsync(i => i.Id == inv)) ?? new Invoice { InvoiceNumber = "NOTFOUND" };

            }
            else
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Invalid invoice identifier.", "OK");
                return;
                // Show error message
            }
            if (invoice.InvoiceNumber == "NOTFOUND")
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Not Found", "No invoice found matching the provided identifier.", "OK");
                return;
            }

            // await Shell.Current.GoToAsy

            // Navigate to Edit Invoice Page with the loaded invoice details
            await Shell.Current.GoToAsync($"EditInvoicePage?InvoiceId={invoice.Id}");

        }
    }

}
