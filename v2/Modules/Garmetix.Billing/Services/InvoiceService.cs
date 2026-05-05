
using DocumentFormat.OpenXml.Drawing;
using Garmetix.Billing.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Billing.Services
{
    public partial class BillingService
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

            public InvoiceService()
            {
                _instance = this;
            }
            private bool isSaving = false;


            public bool UpdateInvoices(Invoice invoice) { return false; }
            public bool UpdateInvoices(InvoiceDTO invoice, IEnumerable<EntryItem> invoiceitems, IEnumerable<PaymentDetail> paymentDetails) { return false; }


            public bool DeleteInvoices(Invoice invoice, bool delete = false) { return true; }


            public Invoice FetchInvoices(Guid storeid, string invnumber) { return new Invoice { InvoiceNumber = "" }; }
            public Invoice FetchInvoices(Guid storeid, Guid InvId) { return new Invoice { InvoiceNumber = "" }; }

            // --- DATABASE SAVE ENGINE ---

            private void CalculateInvoiceTotals(Invoice inv) { }
            public async Task<bool> SaveInvoicesAsync(Invoice invoice) { return false; }
            public async Task<bool> SaveInvoicesAsync(InvoiceDTO invoicedto, IEnumerable<EntryItem> InvoiceItems, IEnumerable<PaymentDetail> paymentDetails)
            {
                if (InvoiceItems.Count() == 0) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cannot save empty invoice.", "OK"); return false; }

                if (isSaving) return false;

                try
                {
                    isSaving = true;

                    var currentInvoice = new Invoice
                    {
                        InvoiceNumber = await GenerateNextInvoiceNumberAsync(),
                        CustomerGSTIN = invoicedto.CustomerGSTIN,
                        ItemCount = InvoiceItems.Count(),

                        ActualQuantity = invoicedto.BilledQuantity,

                        B2BSale = invoicedto.CustomerGSTIN != "" ? true : false,
                        BillAmount = invoicedto.GrandTotal,
                        CustomerMobileNumber = invoicedto.CustomerMobileNumber,
                        Synced = false

                    };

                    CalculateInvoiceTotals(currentInvoice); //do at invoice model and re do here for verification

                    if (currentInvoice.PaidAmount < currentInvoice.BillAmount)
                    {
                        bool proceed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Part Payment", $"Balance of ₹ {CurrentInvoice.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                        if (!proceed) return false;
                    }

                    //await _database.RunInTransactionAsync(tran =>
                    //{
                    //    tran.Insert(CurrentInvoice);
                    //    foreach (var item in InvoiceItems) { item.InvoiceId = CurrentInvoice.Id; tran.Insert(item); }
                    //});


                     


                    try
                    {
                        await GetContext().Database.BeginTransactionAsync();
                        await GetContext().Invoices.AddAsync(currentInvoice);
                        foreach (var item in InvoiceItems) 
                        { 
                            
                            GetContext().InvoiceItems.Add(new InvoiceItem { 
                            Barcode = item.Barcode, Id = item.Id, 
                            CompanyId=currentInvoice.CompanyId, BilledQuantity= item.BilledQuantity,
                            Deleted=false, Synced = false, 
                            ActualQuantity= invoicedto.BilledQuantity, CreatedAt=currentInvoice.CreatedAt, 
                            UpdatedAt=currentInvoice.UpdatedAt,CreatedBy=currentInvoice.CreatedBy,
                            Amount=item.TotalAmount, DiscountAmount=item.DiscountAmount,
                            BasePrice=item.BasePrice, Category=item.Category,
                            InvoiceId=currentInvoice.Id,
                            }); 
                        }

                        // check if this required
                        await GetContext().SaveChangesAsync();
                        await GetContext().Database.CommitTransactionAsync();

                    }
                    catch (Exception)
                    {

                        await GetContext().Database.RollbackTransactionAsync();
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


            public async Task SaveAndPrint(Invoice currentInvoice, IEnumerable<EntryItem> InvoiceItems, IEnumerable<PaymentDetail> paymentDetails, bool thermal = true, bool sendOverMsg = false)
            {
                if (await SaveInvoicesAsync(currentInvoice, InvoiceItems, paymentDetails))
                {
                    if (!thermal)
                    {// Print A5
                        string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                        await Launcher.Default.OpenAsync(new OpenFileRequest { Title = "Print Invoice", File = new ReadOnlyFile(pdfPath) });

                    }
                    if (thermal)
                    {
                        byte[] thermalBytes = ReceiptBuilder.GenerateThermalReceiptBytes(CurrentInvoice, InvoiceItems);
                        await _printService.PrintReceiptAsync(thermalBytes);
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Thermal Receipt Printed.", "OK");

                    }
                    if (sendOverMsg)
                    {
                        if (string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber))
                        {
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Please enter a customer mobile number.", "OK");
                            return;
                        }
                        string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                        string whatsappNumber = CurrentInvoice.CustomerMobileNumber.Length == 10 ? $"91{CurrentInvoice.CustomerMobileNumber}" : CurrentInvoice.CustomerMobileNumber;
                        string message = $"Hello {CurrentInvoice.CustomerName}, thank you for shopping at Aadwika Fashion! Your invoice amount is ₹{CurrentInvoice.GrandTotal:F2}.";
                        string url = $"https://api.whatsapp.com/send?phone={whatsappNumber}&text={Uri.EscapeDataString(message)}";

                        try
                        {
                            await Launcher.Default.OpenAsync(new Uri(url));
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("WhatsApp", "Opening WhatsApp. Please tap 'Attach' to send the generated PDF.", "OK");
                        }
                        catch (Exception) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Could not open WhatsApp.", "OK"); }


                    }
                    ResetFormWithoutPrompt(); //Call at viewmodel
                }

            }


            public async Task SaveAndPrint(InvoiceDTO currentInvoice, IEnumerable<EntryItem> InvoiceItems, IEnumerable<PaymentDetail> paymentDetails, bool thermal = true, bool sendOverMsg = false)
            {
                if (await SaveInvoicesAsync(currentInvoice, InvoiceItems, paymentDetails))
                {
                    if (!thermal)
                    {// Print A5
                        string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                        await Launcher.Default.OpenAsync(new OpenFileRequest { Title = "Print Invoice", File = new ReadOnlyFile(pdfPath) });

                    }
                    if (thermal)
                    {
                        byte[] thermalBytes = ReceiptBuilder.GenerateThermalReceiptBytes(CurrentInvoice, InvoiceItems);
                        await _printService.PrintReceiptAsync(thermalBytes);
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Thermal Receipt Printed.", "OK");

                    }
                    if (sendOverMsg)
                    {
                        if (string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber))
                        {
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Please enter a customer mobile number.", "OK");
                            return;
                        }
                        string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                        string whatsappNumber = CurrentInvoice.CustomerMobileNumber.Length == 10 ? $"91{CurrentInvoice.CustomerMobileNumber}" : CurrentInvoice.CustomerMobileNumber;
                        string message = $"Hello {CurrentInvoice.CustomerName}, thank you for shopping at Aadwika Fashion! Your invoice amount is ₹{CurrentInvoice.GrandTotal:F2}.";
                        string url = $"https://api.whatsapp.com/send?phone={whatsappNumber}&text={Uri.EscapeDataString(message)}";

                        try
                        {
                            await Launcher.Default.OpenAsync(new Uri(url));
                            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("WhatsApp", "Opening WhatsApp. Please tap 'Attach' to send the generated PDF.", "OK");
                        }
                        catch (Exception) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Could not open WhatsApp.", "OK"); }


                    }
                    ResetFormWithoutPrompt(); //Call at viewmodel
                }

            }




        }
    }
}