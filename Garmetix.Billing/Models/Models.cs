using Garmetix.Models.Bharat.Enums;
using Garmetix.Models.Enums;

namespace  Garmetix.Billing.Models;

internal class InvoiceItem
{
    public Guid Id { get; set; }
    public string Barcode { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }

    public decimal Rate { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Tax { get{ return ((Rate * Quantity) - (Rate * Quantity) * (Discount / 100)) * (TaxRate / 100); } }
    public decimal BasePrice { get{ return (Rate * Quantity) - (Rate * Quantity) * (Discount / 100); } }
    public decimal Total { get { return BasePrice + Tax; } }
    public decimal MRP => Rate * Quantity;
    public Unit Unit { get; set; }
}

internal class EntryItem
{
    public Guid Guid { get; set; } 
    public string Barcode { get; set; }
    public string Name { get; set; }
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
}

internal class Party
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string? Email { get; set; }
    public string? GSTIN { get; set; }
    public string? State { get; set; }
    public bool Registered { get; set; }=false;

}

internal class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public Party Party { get; set; }
    public List<InvoiceItem> Items { get; set; }

    public int Count => Items.Count;
    public decimal Quantity => Items.Sum(x => x.Quantity);
    public decimal Total => Items.Sum(x => x.Total);
    public decimal TotalTax => Items.Sum(x => x.Tax);
    public decimal TotalBasicPrice => Items.Sum(x => x.BasePrice);
    public decimal TotalDiscount => Items.Sum(x => (x.Rate * x.Quantity) * (x.Discount / 100));

    public bool IsRegistered => Party.Registered;
    public bool IsRetunInvoice { get; set; } = false;

    public decimal MRP => Items.Sum(x => x.MRP);
    public List<PaymentDetail>? Payments { get; set; } = new List<PaymentDetail>();
}


public class PaymentDetail
{
    public Guid Guid { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMode PaymentMode {  get; set; }=PaymentMode.Cash;
    public string? PaymentNote { get; set; }

    public string? CardPaymentDetails { get; set; }
}
