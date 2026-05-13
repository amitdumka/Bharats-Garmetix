using Garmetix.Billing.Models;
using Garmetix.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Garmetix.Billing.Alternatives;



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

 


