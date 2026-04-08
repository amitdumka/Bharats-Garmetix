// Models/InvoiceModel.cs
namespace Garmetix.Reports.InvoicePrinter.Models
{
    public class InvoiceModel
    {
        public CompanyInfo SellerInfo { get; set; }
        public CompanyInfo BuyerInfo { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<InvoiceItem> Items { get; set; }

        // Calculated properties for GST
        public decimal SubTotal => Items.Sum(i => i.Total);

        public decimal DiscountTotal => Items.Sum(i => i.DiscountAmount);

        // Assuming a simple GST structure for demonstration.
        // In a real app, this might be more complex.
        public decimal CgstRate { get; set; } = 9; // Example 9%

        public decimal SgstRate { get; set; } = 9; // Example 9%
        public decimal IgstRate { get; set; } = 0;  // Use IGST if buyer is in another state

        public decimal CgstAmount => (IgstRate == 0) ? SubTotal * (CgstRate / 100m) : 0;
        public decimal SgstAmount => (IgstRate == 0) ? SubTotal * (SgstRate / 100m) : 0;
        public decimal IgstAmount => SubTotal * (IgstRate / 100m);

        public decimal TaxTotal => CgstAmount + SgstAmount + IgstAmount;
        public decimal GrandTotal => SubTotal + TaxTotal;
    }

    public class CompanyInfo
    {
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gstin { get; set; }
        public string PlaceOfSupply { get; set; }=" 20-Jharkhand";
    }

    public class InvoiceItem
    {
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public string StyleCode { get; set; }
        public string HsnCode { get; set; }
        public decimal Rate { get; set; }
        public string Unit { get; set; } = "Mtrs";
        public int Quantity { get; set; }
        public decimal DiscountPercentage { get; set; }

        public decimal DiscountAmount => (Rate * Quantity) * (DiscountPercentage / 100m);
        public decimal Total => (Rate * Quantity) - DiscountAmount;
    }
}