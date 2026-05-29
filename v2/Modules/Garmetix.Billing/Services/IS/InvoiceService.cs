using Bharat.ToolKits.Notifications;
using Garmetix.Billing.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

//TODO: Check for use of SKIA over Zxing Direct for barcode generation. SKIA might offer better performance and customization options, while ZXing is more straightforward for standard barcode formats. Consider the trade-offs based on your specific needs and constraints.
using SkiaSharp;
using ZXing;
using ZXing.SkiaSharp;

namespace Garmetix.Billing.Services
{
    public enum InvoiceCodeType
    {
        QRCode,
        Barcode1D
    }

   

    //Imlementing Extra and Spl method to handle invoicing
    public partial class InvoiceService : BaseInvoiceService
    {
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

                invoice = (await GetContext().Invoices.Include(i => i.InvoiceItems).Include(i => i.Payments).Include(i => i.CardPayments).FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber))??new Invoice { InvoiceNumber="NOTFOUND"};
            }
            else if (inv != null)
            {

                invoice = (await GetContext().Invoices.Include(i => i.InvoiceItems).Include(i => i.Payments).Include(i => i.CardPayments).FirstOrDefaultAsync(i => i.Id == inv))?? new Invoice { InvoiceNumber = "NOTFOUND" };

            }
            else
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Invalid invoice identifier.", "OK");
                return;
                // Show error message
            }
            if(invoice.InvoiceNumber == "NOTFOUND")
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Not Found", "No invoice found matching the provided identifier.", "OK");
                return;
            }

           // await Shell.Current.GoToAsy

            // Navigate to Edit Invoice Page with the loaded invoice details
            await Shell.Current.GoToAsync($"EditInvoicePage?InvoiceId={invoice.Id}");

        }
    }
        // Add and update record
        public partial class InvoiceService : BaseInvoiceService
    {

        /// <summary>
        /// Add or update customer due based on the invoice balance amount. If balance amount is zero or less, it will remove the due record if exists. If balance amount is greater than zero, it will add or update the due record.
        /// </summary>
        /// <param name="invoice">The invoice for which to add or update customer due</param>
        /// <returns>true if the customer due was added or updated successfully, false otherwise</returns>
        public async Task<bool> AddOrUpdateCustomerDue(Invoice invoice)
        {

            if (invoice is null) return false;


            var existingDue = await GetContext().CustomerDues.FirstOrDefaultAsync(d => d.Id == invoice.Id);

            // Remove due if balance amount is zero or less. 
            if (existingDue != null && invoice.BalanceAmount <= 0)
            {
                GetContext().CustomerDues.Remove(existingDue);
                //TODO: Add log for this action
                return await GetContext().SaveChangesAsync() > 0;
            }

            if (existingDue != null)
            {
                // Update existing due
                existingDue.Amount = invoice.BalanceAmount;
                existingDue.UpdatedAt = DateTime.UtcNow;
                existingDue.CreatedBy = DatabaseService.Instance.CurrentUser.Name;
                GetContext().CustomerDues.Update(existingDue);
            }
            else
            {
                // Create new due and add
                var due = new CustomerDue
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    OnDate = invoice.OnDate,
                    Amount = invoice.BalanceAmount,
                    Paid = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    CompanyId = DatabaseService.CompanyId,
                    ClearingDate = null,
                    Deleted = false,
                    Synced = false,
                    StoreId = DatabaseService.StoreId,
                    StoreGroupId = DatabaseService.StoreGroupId,

                };
                // Add new due
                await GetContext().CustomerDues.AddAsync(due);
            }
            return await GetContext().SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Dispose the object and clean up
        /// </summary>
        public void Dispose()
        {
            GetContext().Dispose();
        }

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



        /// <summary>
        /// Save the invoice and print and can send over message
        /// </summary>
        /// <param name="currentInvoice"></param>
        /// <param name="InvoiceItems"></param>
        /// <param name="paymentDetails"></param>
        /// <param name="print"></param>
        /// <param name="thermal"></param>
        /// <param name="sendOverMsg"></param>
        /// <returns></returns>
        public async Task<bool> SaveAndPrint(InvoiceDTO currentInvoice, IEnumerable<EntryItem> InvoiceItems, IEnumerable<PaymentDetail> paymentDetails, bool print = true, bool thermal = true, bool sendOverMsg = false)
        {
            if (await SaveInvoicesAsync(currentInvoice, InvoiceItems, paymentDetails))
            {
                return await SaveAndPrint(_lastSavedInvoice, _lastSaveditems, _lastSavedPayments, _lastSavedCardPayments, print, thermal, sendOverMsg);
            }
            else
                return false;
        }

        /// <summary>
        /// Save and Print and Can send message to customer
        /// </summary>
        /// <param name="currentInvoice"></param>
        /// <param name="InvoiceItems"></param>
        /// <param name="paymentDetails"></param>
        /// <param name="thermal"></param>
        /// <param name="sendOverMsg"></param>
        /// <returns></returns>

        public async Task<bool> SaveAndPrint(Invoice? currentInvoice, IEnumerable<InvoiceItem>? InvoiceItems, IEnumerable<InvoicePayment>? paymentDetails, IEnumerable<CardPayment>? cardPayments, bool print = true, bool thermal = true, bool sendOverMsg = false)
        {
            string pdfPath = string.Empty;
            bool result = _isSaved || await SaveInvoicesAsync(currentInvoice, InvoiceItems, paymentDetails, cardPayments);
            try
            {
                if (result)
                {
                    if (print)
                    {
                        if (!thermal)
                        {
                            // Print A5
                            pdfPath = PdfReceiptBuilder.GenerateA5Pdf(_lastSavedInvoice, _lastSaveditems, _lastSavedPayments);
                            await Launcher.Default.OpenAsync(new OpenFileRequest { Title = "Print Invoice", File = new ReadOnlyFile(pdfPath) });
                            result = true;
                        }

                        else // if (thermal)
                        {
                            byte[] thermalBytes = ReceiptBuilder.GenerateThermalReceiptBytes(_lastSavedInvoice, _lastSaveditems);
                            await _printService.PrintReceiptAsync(thermalBytes);
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Thermal Receipt Printed.", "OK");
                            result = true;
                        }
                    }
                    if (sendOverMsg)
                    {
                        if (string.IsNullOrWhiteSpace(_lastSavedInvoice?.CustomerMobileNumber))
                        {
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Please enter a customer mobile number.", "OK");
                            return false;
                        }

                        pdfPath ??= PdfReceiptBuilder.GenerateA5Pdf(_lastSavedInvoice, _lastSaveditems, _lastSavedPayments);

                        string whatsappNumber = _lastSavedInvoice.CustomerMobileNumber.Length == 10 ? $"91{_lastSavedInvoice.CustomerMobileNumber}" : _lastSavedInvoice.CustomerMobileNumber;
                        string message = $"Hello {_lastSavedInvoice.CustomerName}, thank you for shopping at Aadwika Fashion! Your invoice amount is ₹{_lastSavedInvoice.BillAmount:F2}.";
                        string url = $"https://api.whatsapp.com/send?phone={whatsappNumber}&text={Uri.EscapeDataString(message)}";

                        try
                        {
                            await Launcher.Default.OpenAsync(new Uri(url));
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("WhatsApp", "Opening WhatsApp. Please tap 'Attach' to send the generated PDF.", "OK");
                            result = true;
                        }
                        catch (Exception)
                        {
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Could not open WhatsApp.", "OK");
                            result = false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Save Invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="invoiceItems"></param>
        /// <param name="paymentDetails"></param>
        /// <param name="cardPayments"></param>
        /// <returns></returns>
        public async Task<bool> SaveInvoicesAsync(Invoice invoice, IEnumerable<InvoiceItem> invoiceItems, IEnumerable<InvoicePayment> paymentDetails, IEnumerable<CardPayment>? cardPayments)
        {
            // Saving Invoice First

            try
            {
                //Handling few check for FK  integrity and other DB related issues before saving the invoice

                //Customer
                invoice.CustomerId = await GetCustomerIdOrDefaultAsync(invoice.CustomerMobileNumber);
                //GetContext().Customers.Where(c => c.MobileNumber == invoice.CustomerMobileNumber).Select(c => c.Id).FirstOrDefaultAsync();

                //Handlinh Salesman
                invoice.SalemanId = await GetContext().Salesmen.Where(s => s.Name == invoice.CreatedBy).Select(s => s.Id).FirstOrDefaultAsync();

                if (invoice.SalemanId == Guid.Empty)
                {
                    invoice.SalemanId = await GetContext().Salesmen.Select(s => s.Id).FirstOrDefaultAsync();
                }

                // handling GST
                if (!invoice.B2BSale && invoice.CustomerGSTIN != null && invoice.CustomerGSTIN.Length == 15)
                {
                    invoice.B2BSale = true;
                }

                if (invoice.B2BSale)
                {
                    var statecode = invoice.CustomerGSTIN?.Substring(0, 2);
                    //var localcode=
                    if (StoreInfo.StateCode == invoice.CustomerGSTIN?.Substring(0, 2))
                    {
                        invoice.InterState = false;
                    }
                    else
                    {
                        invoice.InterState = true;
                    }
                }

                await GetContext().Database.BeginTransactionAsync();

                await GetContext().Invoices.AddAsync(invoice);
                await GetContext().InvoiceItems.AddRangeAsync(invoiceItems);
                await GetContext().InvoicePayments.AddRangeAsync(paymentDetails);
                if (cardPayments != null && cardPayments.Count() > 0)
                    await GetContext().CardPayments.AddRangeAsync(cardPayments);

                var count = await GetContext().SaveChangesAsync();
                if (count > 0)
                {
                    await GetContext().Database.CommitTransactionAsync();
                    _lastSavedInvoice = invoice;
                    _lastSaveditems = invoiceItems;
                    _lastSavedPayments = paymentDetails;
                    if (cardPayments != null) _lastSavedCardPayments = cardPayments;
                    _isSaved = true;

                    DatabaseService.Instance.InvalidateCache();
                    invalidateCache = true;
                    //Update the Customer Due 

                    if (invoice.BalanceAmount > 0)
                    {
                        if (!await AddOrUpdateCustomerDue(invoice))
                        {
                            await Notify.DisplaySnackbarAsync("Failed to update customer due. Kindly report admin to check the log");
                        }
                    }
                    
                        //update the stock


                        if (!await UpdateStockRangeAsync(invoice.InvoiceItems.ToList()))
                    {
                        await Notify.DisplaySnackbarAsync("Failded to update the stock. Kindly report admin to check the log");
                        //TODO: Add Log
                    }


                    return true;
                }
            }
            catch (Exception ex)
            {
                // Show alter for error
                await ShowErrorAsync("Save Invoice", ex);
                await GetContext().Database.RollbackTransactionAsync();
                // FASTEST & SAFEST FIX: Clear the change tracker.
                // This instantly detaches all entities that failed to save,
                // preventing the "already tracking one entity" error on the next try.
                GetContext().ChangeTracker.Clear();
                return false;
            }

            return false;
        }

        /// <summary>
        /// Save invoice using DTO
        /// </summary>
        /// <param name="invoicedto"></param>
        /// <param name="InvoiceItems"></param>
        /// <param name="paymentDetails"></param>
        /// <returns></returns>
        public async Task<bool> SaveInvoicesAsync(InvoiceDTO invoicedto, IEnumerable<EntryItem> InvoiceItems, IEnumerable<PaymentDetail> paymentDetails)
        {
            if (InvoiceItems.Count() == 0)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cannot save empty invoice.", "OK");
                return false;
            }

            if (isSaving) return false;

            try
            {
                isSaving = true;
                // Add payment details
                var invoicePaymentList = new List<InvoicePayment>();
                var cardpaymentList = new List<CardPayment>();
                var currentInvoiceItemsList = new List<InvoiceItem>();
                var currentInvoice = new Invoice
                {
                    Id = invoicedto.Id,
                    Synced = false,
                    Deleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-10),

                    InvoiceNumber = await GenerateNextInvoiceNumberAsync(),
                    OnDate = invoicedto.OnDate,

                    CustomerMobileNumber = invoicedto.CustomerMobileNumber,
                    CustomerName = invoicedto.CustomerName,
                    CustomerGSTIN = invoicedto.CustomerGSTIN,

                    ReturnInvoice = false,
                    B2BSale = invoicedto.IsB2BSale,
                    InterState = invoicedto.IsInterStateSale,

                    Quantity = invoicedto.BilledQuantity,
                    ItemCount = InvoiceItems.Count(),

                    BasePrice = invoicedto.SubTotal,
                    DiscountAmount = invoicedto.TotalDiscount,
                    TaxAmount = invoicedto.TotalTax,

                    CGSTAmount = invoicedto.TotalTax / 2m,
                    SGSTAmount = invoicedto.TotalTax / 2m,
                    IGSTAmount = invoicedto.TotalTax,

                    BillDiscountAmount = invoicedto.GlobalDiscountAmount,
                    RoundOff = invoicedto.RoundOffAmount,
                    BillAmount = invoicedto.GrandTotal,

                    PaidAmount = invoicedto.PaidAmount,

                    CreditSale = invoicedto.BalanceAmount > 0 ? true : false,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    CompanyId = DatabaseService.CompanyId
                    , //TODO: Handle this properly
                    MRP = invoicedto.GrandTotal + invoicedto.GlobalDiscountAmount + invoicedto.TotalDiscount
                };

                if (currentInvoice.PaidAmount < currentInvoice.BillAmount)
                {
                    bool proceed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Part Payment", $"Balance of ₹ {invoicedto.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                    if (!proceed) return false;
                }

                try
                {
                    foreach (var item in InvoiceItems)
                    {
                        currentInvoiceItemsList.Add(new InvoiceItem
                        {
                            Deleted = false,
                            Synced = false,

                            CreatedAt = currentInvoice.CreatedAt,
                            UpdatedAt = currentInvoice.UpdatedAt,
                            CreatedBy = currentInvoice.CreatedBy,

                            InvoiceId = currentInvoice.Id,
                            CompanyId = currentInvoice.CompanyId,

                            Id = item.Id,

                            Barcode = item.Barcode,
                            BilledQuantity = item.BilledQuantity,

                            BasePrice = item.BasePrice,
                            DiscountAmount = item.DiscountAmount,
                            Amount = item.TotalAmount,
                            Category = item.Category,
                            TaxAmount = item.TaxAmount,

                            ProductId = item.ProductId,

                            MRP = item.MRP,
                            TaxPercentage = item.GstPercentage,
                            TaxType = currentInvoice.InterState ? TaxType.IGST : TaxType.GST,
                            TaxId = GetTaxIdByType(currentInvoice.InterState ? TaxType.IGST : TaxType.GST, item.GstPercentage),
                        });
                    }

                    // 1. Identify distinct payment modes in the current transaction
                    var distinctModes = paymentDetails.Select(p => p.PaymentMode).Distinct().ToList();

                    // 2. Set overall paymentMode based on the count of distinct modes
                    currentInvoice.PaymentMode = distinctModes.Count > 1
                         ? PaymentMode.MixPayments
                         : distinctModes.FirstOrDefault();

                    foreach (var item in paymentDetails)
                    {
                        if (item.PaymentMode == PaymentMode.Card)
                        {
                            cardpaymentList.Add(new CardPayment
                            {
                                UpdatedAt = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow,
                                Deleted = false,
                                CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                                Synced = false,
                                CompanyId = currentInvoice.CompanyId,

                                InvoiceId = item.InvoiceId,

                                Id = item.Guid,
                                OnDate = item.PaymentDate,

                                Amount = item.Amount,

                                CardNumber = item.CardPaymentNumber.Value,
                                CardType = item.CardType.Value,
                                Card = item.Card.Value,
                                BankName = item.CardPaymentBank,
                                AuthCode = item.AuthCode.Value,
                            });
                        }

                        invoicePaymentList.Add(new InvoicePayment
                        {
                            CompanyId = currentInvoice.CompanyId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                            Deleted = false,
                            Synced = false,

                            InvoiceId = currentInvoice.Id,

                            Id = item.Guid,

                            OnDate = item.PaymentDate,

                            PaymentMode = item.PaymentMode,
                            Amount = item.Amount,

                            ReferenceNumber = item.PaymentNote
                        });
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                // Notify the dashboard that the database has changed!
                //TODO: Enable this DashboardDataService.Instance.InvalidateCache();


                return await SaveInvoicesAsync(currentInvoice, currentInvoiceItemsList, invoicePaymentList, cardpaymentList);
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Save Invoice Error", ex); return false;
            }
            finally
            {
                isSaving = false;
            }
        }

        public async Task<bool> UpdateInvoicesAsync(Invoice invoice, IEnumerable<InvoiceItem> invoiceitems, IEnumerable<InvoicePayment> paymentDetails, IEnumerable<CardPayment> cardPayments)
        {
            using var tran = await GetContext().Database.BeginTransactionAsync();
            try
            {
                GetContext().Invoices.Update(invoice);

                GetContext().InvoiceItems.RemoveRange(await GetContext().InvoiceItems.Where(i => i.InvoiceId == invoice.Id).ToListAsync());
                GetContext().InvoicePayments.RemoveRange(await GetContext().InvoicePayments.Where(i => i.InvoiceId == invoice.Id).ToListAsync());
                GetContext().CardPayments.RemoveRange(await GetContext().CardPayments.Where(i => i.InvoiceId == invoice.Id).ToListAsync());

                await GetContext().InvoiceItems.AddRangeAsync(invoiceitems);
                await GetContext().InvoicePayments.AddRangeAsync(paymentDetails);
                await GetContext().CardPayments.AddRangeAsync(cardPayments);

                await tran.CommitAsync();
                DatabaseService.Instance.InvalidateCache();
                invalidateCache = true;
                return true;
            }
            catch (Exception ex)
            {
                // Show alter for error
                await ShowErrorAsync("Save Invoice", ex);

                await tran.RollbackAsync();
                // throw;
                return false;
            }
        }
    }
}