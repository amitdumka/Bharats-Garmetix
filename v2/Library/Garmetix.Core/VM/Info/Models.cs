using Garmetix.Core.Models.Inventory;
using Invoice = Garmetix.Core.Models.Inventory.Invoice;
using PurchaseInvoice = Garmetix.Core.Models.Inventory.PurchaseInvoice;
using PurchaseInvoiceItem = Garmetix.Core.Models.Inventory.PurchaseInvoiceItem;

namespace Garmetix.Core.VM.Info
{
    /// <summary>
    /// Sale Invoice ViewModel for UI to display Sale Invoice details.
    /// </summary>
    //TODO: Rename to SaleInvoiceDetails or SaleInvoiceViewModel for better clarity.
    //TODO: Check this is required or not, as we can directly use Invoice model for this purpose.
    public class SaleInvoice
    {
        public required Invoice Invoice { get; set; }
        public List<InvoicePayment> Payments { get; set; }=[];
        public List<CardPayment> CardPayments { get; set; }= [];
        public List<InvoiceItem> InvoiceItems { get; set; }= [];
    }

    /// <summary>
    /// Purchase Invoices ViewModel for UI to display Purchase Invoice details.
    /// </summary>
    public class PurchaseInvoices
    {
        public required PurchaseInvoice Invoice { get; set; } 
        public List<PurchaseInvoiceItem> InvoiceItems { get; set; }= [];
        public List<VendorPayment> Payments { get; set; }= [];
    }

}
