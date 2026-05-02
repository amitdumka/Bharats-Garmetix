using Garmetix.Models.Bharat.Enums;
using Garmetix.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Garmetix.Billing.Models;



/// <summary>
/// Represents an individual item within an invoice, including pricing, quantity, tax, and discount information.
/// </summary>
/// <remarks>This class is intended for use in invoice processing scenarios where detailed item-level information
/// is required, such as billing, reporting, or tax calculation. All monetary values are represented as decimals to
/// ensure precision. The class includes calculated properties for base price, tax, total, and maximum retail price
/// (MRP), which are automatically derived from the item's rate, quantity, discount, and tax rate. The class is internal
/// and not intended for use outside the containing assembly.</remarks>
internal class InvoiceItem
{
    public Guid Id { get; set; }

    [Required]
    public string Barcode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = decimal.Zero;
    public string? HSNCode { get; set; } = string.Empty;
    public decimal Rate { get; set; } = decimal.Zero;
    public decimal Discount { get; set; } = decimal.Zero;
    public decimal TaxRate { get; set; } = decimal.Zero;

    [JsonIgnore]
    public decimal Tax { get { return ((Rate * Quantity) - (Rate * Quantity) * (Discount / 100)) * (TaxRate / 100); } }
    [JsonIgnore]
    public decimal BasePrice { get { return (Rate * Quantity) - (Rate * Quantity) * (Discount / 100); } }
    [JsonIgnore]
    public decimal Total { get { return BasePrice + Tax; } }
    [JsonIgnore]
    public decimal MRP => Rate * Quantity;
    public Unit Unit { get; set; }
}


/// <summary>
/// Represents an item entry containing product identification, quantity, pricing, and discount information for a
/// transaction or inventory record.
/// </summary>
internal class EntryItem
{
    public Guid Guid { get; set; }
    [Required]
    public string Barcode { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; } = decimal.Zero;
    public decimal Qty { get; set; } = decimal.Zero;
    public decimal Discount { get; set; } = decimal.Zero;
    public decimal TaxRate { get; set; } = decimal.Zero;
}


/// <summary>
/// Represents a party entity with identifying and contact information, including registration status and tax details.
/// </summary>
/// <remarks>This class is typically used to store and transfer information about a business or individual party,
/// such as a customer or supplier, within business applications. All properties are mutable to allow for editing party
/// details as needed.</remarks>
internal class Party
{
    public Guid Guid { get; set; } = Guid.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? GSTIN { get; set; } = string.Empty;
    public string? State { get; set; } = string.Empty;
    public bool Registered { get; set; } = false;

}

internal class Invoice
{
    public Guid Id { get; set; } = new Guid();
    [Required]
    public string InvoiceNumber { get; set; } = string.Empty;//TODO: Generate unique invoice number auto inovice number generator
    public DateTime Date { get; set; } = DateTime.Now;
    public Party Party { get; set; } = new Party();
    public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    [JsonIgnore]
    public int Count => Items.Count;

    [JsonIgnore]
    public decimal Quantity => Items.Sum(x => x.Quantity);
    [JsonIgnore]
    public decimal Total => Items.Sum(x => x.Total);
    [JsonIgnore]
    public decimal TotalTax => Items.Sum(x => x.Tax);
    [JsonIgnore]
    public decimal TotalBasicPrice => Items.Sum(x => x.BasePrice);
    [JsonIgnore]
    public decimal TotalDiscount => Items.Sum(x => (x.Rate * x.Quantity) * (x.Discount / 100));

    [JsonIgnore]
    public bool IsRegistered => Party.Registered;
    [Required]
    public bool IsRetunInvoice { get; set; } = false;

    [JsonIgnore]
    public decimal MRP => Items.Sum(x => x.MRP);

    public List<PaymentDetail>? Payments { get; set; } = new List<PaymentDetail>();
}

/// <summary>
/// Represents the details of a payment associated with an invoice, including amount, date, payment mode, and related
/// metadata.
/// </summary>
/// <remarks>Use this class to store or transfer information about individual payments made toward invoices. The
/// properties capture essential payment attributes such as the invoice reference, payment amount, date, mode, and
/// optional notes. Some properties may be relevant only for specific payment modes (for example, card details for card
/// payments).</remarks>
internal class PaymentDetail
{
    public Guid Guid { get; set; } = Guid.Empty;
    public Guid InvoiceId { get; set; } = Guid.Empty;
    public string? InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; } = decimal.Zero;
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
    public string? PaymentNote { get; set; } = string.Empty;

    public string? CardPaymentDetails { get; set; } = string.Empty;
    public string? CardPaymentNumber { get; set; } = string.Empty;
    public string? CardPaymentBank { get; set; } = string.Empty;
    public CardType? Card { get; set; } = CardType.Debit;
}



internal class BillingSettings
{
    public Guid Guid { get; set; } = Guid.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string? CompanyPhone { get; set; } = string.Empty;
    public string? CompanyEmail { get; set; } = string.Empty;
    public string? CompanyGSTIN { get; set; } = string.Empty;
    public string? CompanyState { get; set; } = string.Empty;
}