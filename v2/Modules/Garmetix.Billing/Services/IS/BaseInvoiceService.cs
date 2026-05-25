using Bharat.ToolKits.Helpers;
using Garmetix.Core.Models.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Billing.Services
{
    public partial class BaseInvoiceService : BillingService
    {
        protected IPrintService _printService;

        //Invoice Caching  so no need to requery or fetch again

        protected List<Invoice> _allInvoices = new();
        protected List<InvoicePayment> _allPayments = new();
        protected List<CardPayment> _allCards = new();

        protected bool invalidateCache = false;

        // Last Invoice or Current Invoice So it can hold data
        protected Invoice? _lastSavedInvoice;

        protected IEnumerable<InvoiceItem>? _lastSaveditems;
        protected IEnumerable<InvoicePayment>? _lastSavedPayments;
        protected IEnumerable<CardPayment>? _lastSavedCardPayments;

        protected bool _isSaved = false;
        protected bool isSaving = false;

        public BaseInvoiceService() : base()
        {
            _printService = ServiceHelper.GetService<IPrintService>();
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
    }
}