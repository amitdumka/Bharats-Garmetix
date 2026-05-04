
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
        public class InvoiceService
        {
            private PrintService _printService;

            private InvoiceService _instance;
            public InvoiceService Instance => _instance ?? new InvoiceService();

            public InvoiceService()
            {
                _instance = this;
            }


            public bool SaveInvoices(Invoice invoice){ return false; }
            public bool SaveInvoices(InvoiceDTO invoice, IEnumerable<EntryItem> invoiceitems, IEnumerable<PaymentDetail> paymentDetails) { return false;}
            public bool UpdateInvoices(Invoice invoice){ return false; }
            public bool UpdateInvoices(InvoiceDTO invoice, IEnumerable<EntryItem> invoiceitems, IEnumerable<PaymentDetail> paymentDetails) { return false;}


            public bool DeleteInvoices(Invoice invoice, bool delete=false){ return true; }

             
            public Invoice FetchInvoices(Guid storeid, string invnumber) { }
            public Invoice FetchInvoices(Guid storeid, Guid InvId) { }

            // --- DATABASE SAVE ENGINE ---
            private async Task<bool> SaveInvoiceToDatabaseAsync()
            {
                if (InvoiceItems.Count == 0) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cannot save empty invoice.", "OK"); return false; }
                if (IsBusy) return false;

                try
                {
                    IsBusy = true;
                    //CurrentInvoice.InvoiceNo = "INV-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                    CurrentInvoice.InvoiceNumber = await GenerateNextInvoiceNumberAsync();
                    CalculateInvoiceTotals();

                    if (CurrentInvoice.PaidAmount < CurrentInvoice.GrandTotal)
                    {
                        bool proceed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Part Payment", $"Balance of ₹ {CurrentInvoice.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                        if (!proceed) return false;
                    }

                    //await _database.RunInTransactionAsync(tran =>
                    //{
                    //    tran.Insert(CurrentInvoice);
                    //    foreach (var item in InvoiceItems) { item.InvoiceId = CurrentInvoice.Id; tran.Insert(item); }
                    //});


                    var inv = new Invoice
                    {
                        CustomerGSTIN = CurrentInvoice.CustomerGSTIN,
                        ItemCount = InvoiceItems.Count,
                        InvoiceNumber = CurrentInvoice.InvoiceNumber,
                        ActualQuantity = CurrentInvoice.BilledQuantity,

                        B2BSale = currentInvoice.CustomerGSTIN != "" ? true : false,
                        BillAmount = currentInvoice.GrandTotal,
                        CustomerMobileNumber = currentInvoice.CustomerMobileNumber,
                        Synced = false
                    };


                    try
                    {
                        await GetContext().Database.BeginTransactionAsync();
                        await GetContext().Invoices.AddAsync(inv);
                        foreach (var item in InvoiceItems) { item.InvoiceId = CurrentInvoice.Id; GetContext().InvoiceItems.Add(item); }

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
                finally { IsBusy = false; }
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
            private async Task<string> GenerateNextInvoiceNumberAsync()
            {
                //TODO: move to Invoice Service  even save and delete also . 
                // 1. Get Store Code from MAUI Preferences (Defaults to "AFA" if not set yet)
                string storeCode = Microsoft.Maui.Storage.Preferences.Default.Get("StoreCode", "AFA");

                // 2. Get Current Year and Month (e.g., "202604")
                string yearMonth = DateTime.Now.ToString("yyyyMM");

                // 3. Define the prefix (e.g., "AFA-202604-IN-")
                string prefix = $"{storeCode}-{yearMonth}-IN-";

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
            public async Task SaveAndWhatsAppAsync()
            {
                if (string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber))
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Please enter a customer mobile number.", "OK");
                    return;
                }

                if (await SaveInvoiceToDatabaseAsync())
                {
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

                    ResetFormWithoutPrompt();
                }
            }
            public async Task SaveAndPrintThermalAsync(Invoice CurrentInvoice, List<InvoiceItem> InvoiceItems)
            {
                if (await SaveInvoices(CurrentInvoice, Invoiceitems))
                {
                    byte[] thermalBytes = ReceiptBuilder.GenerateThermalReceiptBytes(CurrentInvoice, InvoiceItems);
                    await _printService.PrintReceiptAsync(thermalBytes);
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Thermal Receipt Printed.", "OK");
                    ResetFormWithoutPrompt();
                }
            }
            public async Task SaveAndPrintThermalAsync(InvoiceDTO CurrentInvoice, List<EntryItem> InvoiceItems)
            {
                if (await SaveInvoices())
                {
                    byte[] thermalBytes = ReceiptBuilder.GenerateThermalReceiptBytes(CurrentInvoice, InvoiceItems);
                    await _printService.PrintReceiptAsync(thermalBytes);
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Thermal Receipt Printed.", "OK");
                    ResetFormWithoutPrompt();
                }
            }
            public async Task SaveAndPrintA5Async()
            {
                if (await SaveInvoiceToDatabaseAsync())
                {
                    string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                    await Launcher.Default.OpenAsync(new OpenFileRequest { Title = "Print Invoice", File = new ReadOnlyFile(pdfPath) });
                    ResetFormWithoutPrompt();
                }
            }

        }
    }
}