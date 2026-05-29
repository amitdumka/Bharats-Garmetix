using Bharat.ToolKits.Helpers;
using Garmetix.Billing.Models;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Microsoft.EntityFrameworkCore;
namespace Garmetix.Billing.Services
{
    // Fetching and Querying Invoice and other record and base constructor and instance
    public partial class InvoiceService : BaseInvoiceService
    {
        private static InvoiceService? _instance;
        public static InvoiceService Instance => _instance ?? new InvoiceService();
        /// <summary>
        /// Default Constructor
        ///     It initilized the instance , print Sercice, and context
        /// </summary>
        public InvoiceService()
        {
            _instance = this;
            _printService = ServiceHelper.GetService<IPrintService>();
            isSaving = false;
            _isSaved = false;
        }

        //Fetching and Querying Invoice and other record
        // -------------Fetching and Query Invoice and other record  //

        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            if (_allInvoices.Any() && !invalidateCache)
            {
                return _allInvoices;
            }

            _allInvoices.Clear();
            _allInvoices = await GetContext().Invoices.Where(x => x.OnDate.Month == DateTime.Now.Month && x.OnDate.Year == DateTime.Now.Year).OrderByDescending(c => c.OnDate.Date).ToListAsync();
            return _allInvoices;
        }

        public async Task<List<InvoicePayment>> GetPaymentsAsync()
        {
            if (_allPayments.Any() && !invalidateCache)
            {
                return _allPayments;
            }

            _allPayments.Clear();
            _allPayments = await GetContext().InvoicePayments.Where(static x => x.OnDate.Month == DateTime.Now.Month && x.OnDate.Year == DateTime.Now.Year).OrderByDescending(c => c.OnDate.Date).ToListAsync();
            return _allPayments;
        }

        public async Task<List<CardPayment>> GetCardPaymentsAsync()
        {
            if (_allCards.Any() && !invalidateCache)
            {
                return _allCards;
            }

            _allCards.Clear();
            _allCards = await GetContext().CardPayments.Where(static x => x.OnDate.Month == DateTime.Now.Month && x.OnDate.Year == DateTime.Now.Year).OrderByDescending(c => c.OnDate.Date).ToListAsync();
            return _allCards;
        }

        public async Task<HashSet<Guid>> GetPayments(PaymentMode mode)
        {
            var validIds = _allPayments
                 .Where(p => p.PaymentMode == mode)
                 .Select(p => p.InvoiceId)
                 .ToHashSet();

            return validIds;
        }

        public async Task<List<Core.Models.Inventory.InvoiceItem>> GetInvoiceItemByInvoiceId(Guid id)
        {
            return await GetContext().InvoiceItems.Where(i => i.InvoiceId == id).ToListAsync();
        }

        /// <summary>
        /// Fetch Invoice by Id
        /// </summary>
        /// <param name="InvId"> Invoice Id , Type is Guid</param>
        /// <returns>Invoice Object or Null</returns>
        public async Task<Invoice?> GetInvoiceByIdAsync(Guid? InvId)
        {
            // Fail fast if ID is null or empty
            if (InvId == null || InvId == Guid.Empty)
                return null;

            try
            {
                var context = GetContext(); // Call this once to reduce overhead
                var invoice = await context.Invoices.FirstOrDefaultAsync(c => c.Id == InvId);

                if (invoice != null)
                {
                    // FIX: Use InvoiceId for child records and ToListAsync() to populate collections safely
                    invoice.InvoiceItems = await context.InvoiceItems
                        .Where(c => c.InvoiceId == InvId)
                        .ToListAsync();

                    invoice.Payments = await context.InvoicePayments
                        .Where(c => c.InvoiceId == InvId)
                        .ToListAsync();

                    if (invoice.PaymentMode == PaymentMode.Card)
                    {
                        invoice.CardPayments = await context.CardPayments
                            .Where(c => c.InvoiceId == InvId)
                            .ToListAsync();
                    }
                }

                return invoice;
            }
            catch (Exception ex)
            {
                // Log the exception (replace Debug with your actual logging mechanism)
                System.Diagnostics.Debug.WriteLine($"Exception in GetInvoiceByIdAsync: {ex.Message}");
                return null;
            }
        }
        public async Task<Invoice?> GetInvoiceByNumberAsync(string? InvNumber)
        {
            // Fail fast if ID is null or empty
            if (InvNumber == null || InvNumber == string.Empty)
                return null;

            try
            {
                var context = GetContext(); // Call this once to reduce overhead
                var invoice = await context.Invoices.FirstOrDefaultAsync(c => c.InvoiceNumber == InvNumber);

                if (invoice != null)
                {
                    // FIX: Use InvoiceId for child records and ToListAsync() to populate collections safely
                    invoice.InvoiceItems = await context.InvoiceItems
                        .Where(c => c.InvoiceId == invoice.Id)
                        .ToListAsync();

                    invoice.Payments = await context.InvoicePayments
                        .Where(c => c.InvoiceId == invoice.Id)
                        .ToListAsync();

                    if (invoice.PaymentMode == PaymentMode.Card)
                    {
                        invoice.CardPayments = await context.CardPayments
                            .Where(c => c.InvoiceId == invoice.Id)
                            .ToListAsync();
                    }
                }

                return invoice;
            }
            catch (Exception ex)
            {
                // Log the exception (replace Debug with your actual logging mechanism)
                System.Diagnostics.Debug.WriteLine($"Exception in GetInvoiceByNumberAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<InvoiceDTO?> GetInvoiceDTOById(Guid id)
        {
            try
            {
                var inv = await GetInvoiceByIdAsync(id);

                // FIX: Prevent NullReferenceException if the invoice does not exist
                if (inv == null)
                    return null;

                return inv.ToInvoiceDto();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in GetInvoiceDTOById: {ex.Message}");
                return null;
            }
        }

        public async Task<List<EntryItem>> GetEntryItemListAsync(Guid id)
        {
            try
            {
                var items = await GetInvoiceItemByInvoiceId(id);

                // Safely handle null results from the database fetch
                if (items == null || !items.Any())
                    return new List<EntryItem>();

                // Optimized using LINQ instead of a foreach loop
                return items.Select(item => item.ToEntryItem() ?? new EntryItem()).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in GetEntryItemListAsync: {ex.Message}");
                return new List<EntryItem>(); // Returning an empty list is safer for UIs than returning null
            }
        }

        public async Task<List<PaymentDetail>> GetPaymentDetailsAsync(Guid id)
        {
            try
            {
                
                var items = await GetContext().InvoicePayments
                    .Where(c => c.InvoiceId == id)
                    .ToListAsync();

                if (items == null || !items.Any())
                    return new List<PaymentDetail>();

                // Optimized using LINQ
                return items.Select(item => item.ToPaymentDetail() ?? new PaymentDetail()).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in GetPaymentDetailsAsync: {ex.Message}");
                return new List<PaymentDetail>(); // Prevents UI crashes if binding to a list
            }
        }

        //---------------End of Fetching-----------------------------//

        /// <summary>
        /// Fetch invoice from Company/StoreId and Invoice Number
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="invnumber"></param>
        /// <returns></returns>
        public Invoice? FetchInvoices(Guid? storeid, string invnumber)
        {
            if (storeid == null || string.IsNullOrEmpty(invnumber)) return null;

            var invoice = GetContext().Invoices.Where(c => c.CompanyId == storeid && c.InvoiceNumber == invnumber).FirstOrDefault();
            if (invoice != null)
            {
                invoice.InvoiceItems = (ICollection<InvoiceItem>)GetContext().InvoiceItems.Where(c => c.CompanyId == storeid && c.Id == invoice.Id).ToAsyncEnumerable();
                invoice.Payments = (ICollection<InvoicePayment>)GetContext().InvoicePayments.Where(c => c.CompanyId == storeid && c.Id == invoice.Id).ToAsyncEnumerable();
                if (invoice.PaymentMode == PaymentMode.Card)
                {
                    invoice.CardPayments = (ICollection<CardPayment>)GetContext().CardPayments.Where(c => c.CompanyId == storeid && c.Id == invoice.Id).ToAsyncEnumerable();
                }
                return invoice;
            }
            return null;
        }

        /// <summary>
        /// Fetch Invoices using Company or Store id and Invoice Id
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="InvId"></param>
        /// <returns></returns>
        public Invoice? FetchInvoices(Guid? storeid, Guid? InvId)
        {
            if (storeid == null || InvId == null) return null;

            var invoice = GetContext().Invoices.Where(c => c.CompanyId == storeid && c.Id == InvId).FirstOrDefault();
            if (invoice != null)
            {
                invoice.InvoiceItems = (ICollection<InvoiceItem>)GetContext().InvoiceItems.Where(c => c.CompanyId == storeid && c.Id == InvId).ToAsyncEnumerable();
                invoice.Payments = (ICollection<InvoicePayment>)GetContext().InvoicePayments.Where(c => c.CompanyId == storeid && c.Id == InvId).ToAsyncEnumerable();
                if (invoice.PaymentMode == PaymentMode.Card)
                {
                    invoice.CardPayments = (ICollection<CardPayment>)GetContext().CardPayments.Where(c => c.CompanyId == storeid && c.Id == InvId).ToAsyncEnumerable();
                }
                return invoice;
            }
            return null;
        }

    }



}
