using Garmetix.Billing.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Billing.Services
{
    //Note: All Save Main Function are here

    public partial class InvoiceService : BaseInvoiceService
    {
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
                    CompanyId = DatabaseService.CompanyId,
                    StoreId = DatabaseService.StoreId,
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

                            Id = Guid.NewGuid(),// item.Id,

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
                            TaxId = BillingService.GetTaxIdByType(currentInvoice.InterState ? TaxType.IGST : TaxType.GST, item.GstPercentage),
                        });
                    }

                    currentInvoice.MRP = currentInvoiceItemsList.Sum(i => i.MRP * i.BilledQuantity);

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
                                StoreId = DatabaseService.StoreId,

                                InvoiceId = currentInvoice.Id,

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
                            StoreId = DatabaseService.StoreId,

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
    }
}

