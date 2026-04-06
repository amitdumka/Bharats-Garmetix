using Garmetix.Models.Inventory;
using Invoice = Garmetix.Models.Inventory.Invoice;
using PurchaseInvoice = Garmetix.Models.Inventory.PurchaseInvoice;
using PurchaseInvoiceItem = Garmetix.Models.Inventory.PurchaseInvoiceItem;

namespace Garmetix.Models.Info
{
    /// <summary>
    /// Sale Invoice ViewModel for UI to display Sale Invoice details.
    /// </summary>
    
    public class SaleInvoice
    {
        public Invoice Invoice { get; set; }
        public List<InvoicePayment> Payments { get; set; }
        public List<CardPayment> CardPayments { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }
    }

    /// <summary>
    /// Purchase Invoices ViewModel for UI to display Purchase Invoice details.
    /// </summary>
    public class PurchaseInvoices
    {
        public PurchaseInvoice Invoice { get; set; }
        public List<PurchaseInvoiceItem> InvoiceItems { get; set; }
        public List<VendorPayment> Payments { get; set; }
    }

}
