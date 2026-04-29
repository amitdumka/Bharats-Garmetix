using Garmetix.Core.Models.Inventory;



namespace Garmetix.Services
{
    /// <summary>
    /// A service to handle GST calculations for an invoice.
    /// </summary>
    public class GstService
    {
        //TODO: Move this to Tookit.GSTServices.

        //TODO: Read from settinng or storage
        // Assume the business is registered in this state.
        // This should be configurable in a settings file.
        private static string BusinessState = Preferences.Get("State", "Jharkhand");

        /// <summary>
        /// Calculates all taxes for a given invoice based on its items and customer state.
        /// </summary>
        /// <param name="invoice">The invoice to calculate taxes for.</param>
        public void CalculateGst(Invoice invoice)
        {
            if (invoice == null || invoice.Customer == null) return;

            // Determine if the sale is interstate
            invoice.InterState = !string.IsNullOrWhiteSpace(invoice.Customer.State) &&
                                   !invoice.Customer.State.Equals(BusinessState, StringComparison.OrdinalIgnoreCase);

            // Reset tax amounts
            invoice.CGSTAmount = 0;
            invoice.SGSTAmount = 0;
            invoice.IGSTAmount = 0;

            foreach (var item in invoice.InvoiceItems)
            {
                var productGstRate = item.Product?.TaxRate ?? 5;
                var taxableValue = item.TaxableAmount;

                if (invoice.InterState)
                {
                    // Apply IGST for interstate transactions
                    invoice.IGSTAmount += taxableValue * (productGstRate / 100);
                }
                else
                {
                    // Apply CGST and SGST for local (intra-state) transactions
                    var halfRate = productGstRate / 2;
                    invoice.CGSTAmount += taxableValue * (halfRate / 100);
                    invoice.SGSTAmount += taxableValue * (halfRate / 100);
                }
            }

            invoice.TaxAmount = invoice.CGSTAmount.Value + invoice.SGSTAmount.Value + invoice.IGSTAmount.Value;
        }
    }
}