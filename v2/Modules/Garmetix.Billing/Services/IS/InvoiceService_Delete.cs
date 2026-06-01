using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garmetix.Billing.Services
{
    // Delete record
    public partial class InvoiceService:BaseInvoiceService
    {
        /// <summary>
        /// Delete invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public async Task<bool> DeleteInvoicesAsync(Invoice invoice, bool delete = false)
        {
            if (invoice == null) return false;

            // Use a 'using' block for the transaction to ensure it disposes correctly
            using var transaction = await GetContext().Database.BeginTransactionAsync();

            try
            {
                if (delete) // Hard Delete
                {
                    GetContext().InvoiceItems.RemoveRange(invoice.InvoiceItems);

                    var payments = await GetContext().InvoicePayments.Where(c => c.InvoiceId == invoice.Id).ToListAsync();
                    if (payments.Any())
                        GetContext().InvoicePayments.RemoveRange(payments);

                    if (invoice.PaymentMode == PaymentMode.Card)
                    {
                        var cpayments = await GetContext().CardPayments.Where(c => c.InvoiceId == invoice.Id).ToListAsync();
                        if (cpayments.Any())
                            GetContext().CardPayments.RemoveRange(cpayments);
                    }

                    // CRITICAL FIX: Delete the actual invoice record
                    GetContext().Invoices.Remove(invoice);
                }
                else // Soft Delete
                {
                    invoice.Deleted = true;
                    invoice.UpdatedAt = DateTime.UtcNow;

                    foreach (var item in invoice.InvoiceItems)
                    {
                        item.Deleted = true;
                        item.UpdatedAt = DateTime.UtcNow;
                    }

                    var payments = GetContext().InvoicePayments.Where(c => c.InvoiceId == invoice.Id).ToList();
                    foreach (var item in payments)
                    {
                        item.Deleted = true;
                        item.UpdatedAt = DateTime.UtcNow;
                    }

                    if (invoice.PaymentMode == PaymentMode.Card)
                    {
                        var cpayments = GetContext().CardPayments.Where(c => c.InvoiceId == invoice.Id).ToList();
                        foreach (var item in cpayments)
                        {
                            item.Deleted = true;
                            item.UpdatedAt = DateTime.UtcNow;
                        }
                        GetContext().CardPayments.UpdateRange(cpayments);
                    }

                    GetContext().InvoiceItems.UpdateRange(invoice.InvoiceItems);
                    GetContext().InvoicePayments.UpdateRange(payments);
                    GetContext().Invoices.Update(invoice);
                }

                await GetContext().SaveChangesAsync();
                await transaction.CommitAsync();

                InvalidateCache(); // Invalidate cache after deletion
                return true;
            }
            catch (Exception ex)
            {
                // Log the error (ex) here
                await transaction.RollbackAsync();
                return false;
            }
        }

        /// <summary>
        /// Delete invoice
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="invId"></param>
        /// <param name="invno"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public async Task<bool> DeleteInvoicesAsync(Guid companyId, Guid? invId, string? invno, bool delete = false)
        {
            var invoice = await GetContext().Invoices.Include(x => x.InvoiceItems).FirstOrDefaultAsync(x => x.CompanyId == companyId && (x.Id == invId || x.InvoiceNumber == invno));
            if (invoice == null) return false;

            return await DeleteInvoicesAsync(invoice, delete);
        }

    }
}
