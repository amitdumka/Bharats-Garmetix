using Bharat.ToolKits.Notifications;
using Garmetix.Billing.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Billing.Services
{
    // Add and update record
    public partial class InvoiceService : BaseInvoiceService
    {
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
            string pdfPath = null;
            bool result = false;

            if (_isSaved)
            {
                result = true;
            }
            else //TODO: Check for null and handle it and implement the null handli
                result = await SaveInvoicesAsync(currentInvoice, InvoiceItems, paymentDetails, cardPayments);

            try
            {



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
                        if (string.IsNullOrWhiteSpace(_lastSavedInvoice?.CustomerMobileNumber))
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
                invoice.CustomerId = await GetContext().Customers.Where(c => c.MobileNumber == invoice.CustomerMobileNumber).Select(c => c.Id).FirstOrDefaultAsync();
                //TODO: if still customer id not found then move to walkin customer  

                //Handlinh Salesman 
                invoice.SalemanId = await GetContext().Salesmen.Where(s => s.Name == invoice.CreatedBy).Select(s => s.Id).FirstOrDefaultAsync();

                if (invoice.SalemanId == Guid.Empty)
                {
                    invoice.SalemanId = await GetContext().Salesmen.Select(s => s.Id).FirstOrDefaultAsync();
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
                DatabaseService.Instance.InvalidateCache();

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
