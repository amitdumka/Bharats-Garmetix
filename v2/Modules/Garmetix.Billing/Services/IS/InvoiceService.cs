using Bharat.ToolKits.Notifications;
using Garmetix.Billing.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

//TODO: Check for use of SKIA over Zxing Direct for barcode generation. SKIA might offer better performance and customization options, while ZXing is more straightforward for standard barcode formats. Consider the trade-offs based on your specific needs and constraints.

namespace Garmetix.Billing.Services
{
    // Add and update record
    public partial class InvoiceService : BaseInvoiceService
    {

        private async Task<bool> HandleStockReturnOnUpdate(Guid storeid, List<InvoiceItem> invoiceItems)
        {
            //TODO: Implement the logic to handle stock return on invoice update. This will involve checking the previous invoice items and comparing them with the updated items to determine if any stock needs to be returned or adjusted.
            //TODO: Move this InvoiceServie
            int count = 0;
            foreach (var item in invoiceItems)
            {

                var stock = await GetContext().Stocks.Where(s => s.ProductId == item.ProductId && s.StoreId == storeid).FirstOrDefaultAsync();

                if (stock != null)
                {

                    stock.SoldQty -= item.BilledQuantity;
                    GetContext().Stocks.Update(stock);
                    count++;
                }
            }
            return count != invoiceItems.Count;

        }

        private async Task HandlePostSaveAndUpdate(Invoice invoice)
        {
            // Update the Customer Due
            DatabaseService.Instance.InvalidateCache();
            invalidateCache = true;
            //Update the Customer Due
            //await ShowErrorAsync("Post save", " Reached here");
            if (invoice.BalanceAmount > 0)
            {
                if (!await AddOrUpdateCustomerDue(invoice))
                {
                    await Notify.DisplaySnackbarAsync("Failed to update customer due. Kindly report admin to check the log");
                }
            }
            // Update the Stock

            //update the stock

            if (!await UpdateStockRangeAsync(invoice.InvoiceItems.ToList()))
            {
                await Notify.DisplaySnackbarAsync("Failded to update the stock. Kindly report admin to check the log");
                //TODO: Add Log
            }

            //TODO: return TaskCompleted;

        }

        private IEnumerable<InvoiceItem> HandleInvoiceItems(ref IEnumerable<InvoiceItem> invoiceItems)
        {
            // Handle the invoice items for any calculation or data integrity before saving to DB
            foreach (var item in invoiceItems)
            {
                // TODO: handle Tax calculation, Tax type and other calculation related to the invoice item
                //TODO: Product Id and Barcode integrity check. If product id is provided then barcode should match with the product and vice versa. Also handle the case when both are not provided or both are provided but not matching
            }
            return invoiceItems;
        }

        private async Task<Invoice> HandleInvoiceBasicOperations(Invoice invoice)
        {
            //Handling few check for FK  integrity and other DB related issues before saving the invoice
            // Handling store and Store Group
            invoice.StoreId = DatabaseService.StoreId;

            //Customer
            invoice.CustomerId = await BillingService.GetCustomerIdOrDefaultAsync(invoice.CustomerMobileNumber);

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

            return invoice;
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

                invoice = await HandleInvoiceBasicOperations(invoice);

                // Handle the invoice items for any calculation or data integrity before saving to DB
                if (invoiceItems.Any())
                {

                    //TODO: invoiceItems = HandleInvoiceItems(ref invoiceItems);
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

                    await HandlePostSaveAndUpdate(invoice);

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
        public async Task<bool> UpdateInvoicesAsync(Invoice invoice, IEnumerable<InvoiceItem> invoiceitems, IEnumerable<InvoicePayment> paymentDetails, IEnumerable<CardPayment> cardPayments)
        {
            var context = GetContext(); // Store in a variable so we aren't calling GetContext() repeatedly
            using var tran = await context.Database.BeginTransactionAsync();
            try
            {
                invoice = await HandleInvoiceBasicOperations(invoice);

                if (invoiceitems.Any())
                {
                    //TODO: invoiceItems = HandleInvoiceItems(ref invoiceItems);
                }

                // --- THE FIX: Detach the old tracked instance if it exists in memory ---
                var trackedInvoice = context.Invoices.Local.FirstOrDefault(e => e.Id == invoice.Id);
                if (trackedInvoice != null)
                {
                    context.Entry(trackedInvoice).State = EntityState.Detached;
                }

                // Now it is safe to update the new invoice
                context.Invoices.Update(invoice);

                // Fetch old items and handle stock
                var oldInvoiceItems = await context.InvoiceItems.Where(i => i.InvoiceId == invoice.Id).ToListAsync();
                var flag = await HandleStockReturnOnUpdate(invoice.StoreId, oldInvoiceItems);

                // Remove old related data
                context.InvoiceItems.RemoveRange(oldInvoiceItems);
                context.InvoicePayments.RemoveRange(await context.InvoicePayments.Where(i => i.InvoiceId == invoice.Id).ToListAsync());
                context.CardPayments.RemoveRange(await context.CardPayments.Where(i => i.InvoiceId == invoice.Id).ToListAsync());

                // Add new related data
                await context.InvoiceItems.AddRangeAsync(invoiceitems);
                await context.InvoicePayments.AddRangeAsync(paymentDetails);
                await context.CardPayments.AddRangeAsync(cardPayments);

                //TODO: Update Due Recovery if balance amount is reduced and update the stock accordingly

                // --- CRITICAL FIX: You must save changes to the DB before committing the transaction ---
                await context.SaveChangesAsync();

                await tran.CommitAsync();
                await HandlePostSaveAndUpdate(invoice);

                return true;
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                context.ChangeTracker.Clear(); // Clears out the messed up state

                await ShowErrorAsync("Save Invoice", ex);
                return false;
            }
        }
        public async Task<bool> UpdateInvoicesAsync_old(Invoice invoice, IEnumerable<InvoiceItem> invoiceitems, IEnumerable<InvoicePayment> paymentDetails, IEnumerable<CardPayment> cardPayments)
        {
            using var tran = await GetContext().Database.BeginTransactionAsync();
            try
            {

                invoice = await HandleInvoiceBasicOperations(invoice);

                // Handle the invoice items for any calculation or data integrity before saving to DB
                if (invoiceitems.Any())
                {

                    //TODO: invoiceItems = HandleInvoiceItems(ref invoiceItems);
                }

                GetContext().Invoices.Update(invoice);


                var oldInvoiceItems = await GetContext().InvoiceItems.Where(i => i.InvoiceId == invoice.Id).ToListAsync();
                var flag = await HandleStockReturnOnUpdate(invoice.StoreId, oldInvoiceItems);
                GetContext().InvoiceItems.RemoveRange(oldInvoiceItems);


                GetContext().InvoicePayments.RemoveRange(await GetContext().InvoicePayments.Where(i => i.InvoiceId == invoice.Id).ToListAsync());
                GetContext().CardPayments.RemoveRange(await GetContext().CardPayments.Where(i => i.InvoiceId == invoice.Id).ToListAsync());

                await GetContext().InvoiceItems.AddRangeAsync(invoiceitems);
                await GetContext().InvoicePayments.AddRangeAsync(paymentDetails);
                await GetContext().CardPayments.AddRangeAsync(cardPayments);

                //TODO: Update Due Recovery if balance amount is reduced and update the stock accordingly


                await tran.CommitAsync();
                await HandlePostSaveAndUpdate(invoice);
                return true;
            }
            catch (Exception ex)
            {
                // Show alter for error
                await ShowErrorAsync("Save Invoice", ex);

                await tran.RollbackAsync();
                GetContext().ChangeTracker.Clear();
                // throw;
                return false;
            }
        }
    }
}