using Bharat.ToolKits.Helpers;
using Garmetix.Billing.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Billing.Services
{
    /// <summary>
    /// invoice Service is used to handle Invoice/Billing Services
    /// It will be extend Billing Service as Base Service.
    /// </summary>
    public class InvoiceService : BillingService
    {
        private IPrintService _printService;

        private InvoiceService _instance;
        public InvoiceService Instance => _instance ?? new InvoiceService();

        // Last Invoice or Current Invoice So it can hold data
        private Invoice _lastSavedInvoice;

        private IEnumerable<InvoiceItem> _lastSaveditems;
        private IEnumerable<InvoicePayment> _lastSavedPayments;
        private IEnumerable<CardPayment> _lastSavedCardPayments;
        private bool _isSaved;

        public InvoiceService()
        {
            _instance = this;

            // getting service from logal
            _printService = ServiceHelper.GetService<IPrintService>();
            if (_printService == null)
            {
            }
        }

        private bool isSaving = false;

        public bool UpdateInvoices(Invoice invoice)
        { return false; }

        public bool UpdateInvoices(InvoiceDTO invoice, IEnumerable<EntryItem> invoiceitems, IEnumerable<PaymentDetail> paymentDetails)
        { return false; }

        public bool DeleteInvoices(Invoice invoice, bool delete = false)
        { return true; }

        public Invoice FetchInvoices(Guid storeid, string invnumber)
        { return new Invoice { InvoiceNumber = "" }; }

        public Invoice FetchInvoices(Guid storeid, Guid InvId)
        { return new Invoice { InvoiceNumber = "" }; }

        // --- DATABASE SAVE ENGINE ---

        private void CalculateInvoiceTotals(Invoice inv)
        { 

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
                await GetContext().Database.BeginTransactionAsync();

                await GetContext().Invoices.AddAsync(invoice);
                await GetContext().InvoiceItems.AddRangeAsync(invoiceItems);
                await GetContext().InvoicePayments.AddRangeAsync(paymentDetails);
                if (cardPayments != null)
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

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Show alter for error
                await ShowErrorAsync("Save Invoice", ex);
                await GetContext().Database.RollbackTransactionAsync();
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
            if (InvoiceItems.Count() == 0) { 
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cannot save empty invoice.", "OK");
                return false; }

            if (isSaving) return false;

            try
            {
                isSaving = true;

                var currentInvoice = new Invoice
                {
                    InvoiceNumber = await GenerateNextInvoiceNumberAsync(),
                    CustomerGSTIN = invoicedto.CustomerGSTIN,
                    B2BSale = invoicedto.IsB2BSale,
                    Synced = false,
                    CustomerMobileNumber = invoicedto.CustomerMobileNumber,
                    ItemCount = InvoiceItems.Count(),
                    ActualQuantity = invoicedto.BilledQuantity,
                    BilledQuantity = invoicedto.BilledQuantity,
                    Id = invoicedto.Id, CustomerName = invoicedto.CustomerName,
                    InterState=invoicedto.IsInterStateSale, 
                    Deleted=false, CreatedAt=DateTime.UtcNow,OnDate=invoicedto.OnDate,                                        
                    BillAmount = invoicedto.GrandTotal, CreditSale=invoicedto.BalanceAmount>0?true:false,
                    TaxAmount = invoicedto.TotalTax, 
                    CGSTAmount=invoicedto.TotalTax/2m, SGSTAmount=invoicedto.TotalTax/2m , IGSTAmount=invoicedto.TotalTax, 
                    BillDiscountAmount=invoicedto.GlobalDiscountAmount, DiscountAmount=invoicedto.TotalDiscount,
                    BasePrice=invoicedto.SubTotal,PaidAmount=invoicedto.PaidAmount, ReturnInvoice=false, 
                    RoundOff=invoicedto.RoundOffAmount, UpdatedAt=DateTime.UtcNow.AddMinutes(-10), 
                    
                 
                };

                CalculateInvoiceTotals(currentInvoice); //do at invoice model and re do here for verification

                if (currentInvoice.PaidAmount < currentInvoice.BillAmount)
                {
                    bool proceed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Part Payment", $"Balance of ₹ {invoicedto.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                    if (!proceed) return false;
                }

                
                try
                {
                   
                    foreach (var item in InvoiceItems)
                    {
                        GetContext().InvoiceItems.Add(new InvoiceItem
                        {
                            Barcode = item.Barcode,
                            Id = item.Id,
                            CompanyId = currentInvoice.CompanyId,
                            BilledQuantity = item.BilledQuantity,
                            Deleted = false,
                            Synced = false,
                            ActualQuantity = invoicedto.BilledQuantity,
                            CreatedAt = currentInvoice.CreatedAt,
                            UpdatedAt = currentInvoice.UpdatedAt,
                            CreatedBy = currentInvoice.CreatedBy,
                            Amount = item.TotalAmount,
                            DiscountAmount = item.DiscountAmount,
                            BasePrice = item.BasePrice,
                            Category = item.Category,
                            InvoiceId = currentInvoice.Id,
                        });
                    }

                    
                    
                }
                catch (Exception ex)
                {
                    throw;
                }

                // Notify the dashboard that the database has changed!
                // DashboardDataService.Instance.InvalidateCache();
                DatabaseService.Instance.InvalidateCache();
                return true;
            }
            catch (Exception ex) { await ShowErrorAsync("Save Invoice Error", ex); return false; }
            finally { isSaving = false; }
        }

        /// <summary>
        /// Generates the next unique invoice number for the current store and month in the format
        /// 'STORECODE-YYYYMM-IN-XXXX'.
        /// </summary>
        /// <remarks>The invoice number is incremented based on the most recent invoice for the current
        /// month and store. If no invoices exist for the current month, the sequence starts at 0001. In case of a
        /// database error, a random four-digit sequence is used as a fallback. The method uses the store code from
        /// application preferences, defaulting to 'AFA' if not set.</remarks>
        /// <returns>A string containing the next invoice number, formatted with the store code, current year and month, and a
        /// four-digit sequence number.</returns>
        public async Task<string> GenerateNextInvoiceNumberAsync()
        {
            //TODO: move to Invoice Service  even save and delete also .
            // 1. Get Store Code from MAUI Preferences (Defaults to "AFA" if not set yet)

            string storeCode = Preferences.Default.Get("StoreCode", "AFA");
            // 3. Define the prefix (e.g., "AFA-202604-IN-")
            string prefix = $"{storeCode}-{DateTime.Now.ToString("yyyyMM")}-IN-";

            try
            {
                // 4. Find the most recent invoice in the database that matches THIS month's prefix
                var lastInvoice = await GetContext().Invoices
                    .Where(i => i.InvoiceNumber.StartsWith(prefix))
                    .OrderByDescending(i => i.InvoiceNumber)
                    .FirstOrDefaultAsync();

                int nextSequenceNumber = 1; // Default to 1 if it's the first bill of the month

                if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
                {
                    // Extract the last 4 characters (the numbers) from the previous invoice
                    string lastSequenceStr = lastInvoice.InvoiceNumber.Substring(lastInvoice.InvoiceNumber.Length - 4);

                    if (int.TryParse(lastSequenceStr, out int lastSequence))
                    {
                        nextSequenceNumber = lastSequence + 1;
                    }
                }

                // 5. Format the number with leading zeros so it is always 4 digits (e.g., "0001")
                string sequenceString = nextSequenceNumber.ToString("D4");

                // 6. Return the perfectly formatted string
                return $"{prefix}{sequenceString}";
            }
            catch (Exception ex)
            {
                // Fallback in case of an unexpected database read error
                System.Diagnostics.Debug.WriteLine($"Error generating invoice number: {ex.Message}");
                string fallbackSequence = new Random().Next(1000, 9999).ToString();
                return $"{prefix}{fallbackSequence}";
            }
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

        public async Task<bool> SaveAndPrint(Invoice currentInvoice, IEnumerable<InvoiceItem> InvoiceItems, IEnumerable<InvoicePayment> paymentDetails, bool print = true, bool thermal = true, bool sendOverMsg = false)
        {
            string pdfPath = null;
            bool result = false;

            if (_isSaved)
            {
                result = true;
            }
            result = await SaveInvoicesAsync(currentInvoice, InvoiceItems, paymentDetails);
            if (result)
            {
                if (!thermal)
                {// Print A5
                    pdfPath = PdfReceiptBuilder.GenerateA5Pdf(_lastSavedInvoice, _lastSaveditems, _lastSavedPayments);
                    await Launcher.Default.OpenAsync(new OpenFileRequest { Title = "Print Invoice", File = new ReadOnlyFile(pdfPath) });
                    result = true;
                }
                if (thermal)
                {
                    byte[] thermalBytes = ReceiptBuilder.GenerateThermalReceiptBytes(_lastSavedInvoice, _lastSaveditems);
                    await _printService.PrintReceiptAsync(thermalBytes);
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Thermal Receipt Printed.", "OK");
                    result = true;
                }
                if (sendOverMsg)
                {
                    if (string.IsNullOrWhiteSpace(_lastSavedInvoice.CustomerMobileNumber))
                    {
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Please enter a customer mobile number.", "OK");
                        return false;
                    }

                    if (pdfPath == null)
                    {
                        pdfPath = PdfReceiptBuilder.GenerateA5Pdf(_lastSavedInvoice, _lastSaveditems, _lastSavedPayments);
                    }

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
            return result;
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
                return await SaveAndPrint(_lastSavedInvoice, _lastSaveditems, _lastSavedPayments, print, thermal, sendOverMsg);
            }
            else
                return false;
        }
    }
}