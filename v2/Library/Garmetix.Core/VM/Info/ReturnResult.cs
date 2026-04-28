using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.VM.Info;

//ViewModels

/// <summary>
/// VendorInfo ViewModel for UI to display Vendor details. and
/// provides Vendor Information for Get Vendor Call For Entry
/// </summary>
public class VendorInfo
{
    [Display(Name = "ID", AutoGenerateField = false)] public Guid Id { get; set; } = Guid.NewGuid();
    [Display(Name = "Vendor Name")] public string VendorName { get; set; } = string.Empty;
    [Display(Name = "GSTIN")] public string GSTIN { get; set; } = string.Empty;
    [Display(Name = "Name")] public string Name { get => VendorName + " (" + GSTIN + ")"; }
}

/// <summary>
/// StockInfo ViewModel for UI to display Stock details.
/// Stock Information for Get Stock Call For Entry
/// </summary>
public class StockInfo
{
    [Display(Name = "Product ID", AutoGenerateField = false)] public Guid ProductId { get; set; }
    [Display(Name = "Barcode")] public string Barcode { get; set; } = string.Empty;
    [Display(Name = "Quantity")] public decimal Quantity { get; set; } = decimal.Zero;
    [Display(Name = "Unit Price")] public decimal UnitPrice { get; set; } = decimal.Zero;
    [Display(Name = "Tax Rate")] public decimal TaxRate { get; set; } = decimal.Zero;
    [Display(Name = "Product Name")] public string? ProductName { get; set; } = string.Empty;
    [Display(Name = "HSN Code")] public string? HSNCode { get; set; } = string.Empty;
    [Display(Name = "Is Discounted")] public bool IsDiscounted { get; set; } = false;
    [Display(Name = "Name")] public string Name { get => ProductName + "-" + Barcode + " (" + HSNCode + ")"; }
}

/// <summary>
/// CustomerInfo ViewModel for UI to display Customer details.
/// </summary>
public class CustomerInfo
{
    [Display(Name = "ID", AutoGenerateField = false)] public Guid Id { get; set; } = Guid.NewGuid();
    [Display(Name = "Name")] public string Name { get; set; } = string.Empty;
    [Display(Name = "Mobile")] public string Mobile { get; set; } = string.Empty;
    [Display(Name = "GSTIN")] public string? GSTIN { get; set; } = null;
    [Display(Name = "Count")] public int Count { get; set; } = 0;

    [Display(Name = "Purchase Value")] public decimal PurchaseValue { get; set; } = 0;
    [Display(Name = "Loyalty Points")] public decimal LoyaltyPoints { get; set; } = 0;
    [Display(Name = "Customer Name")] public string CustomerName { get => Name + " - " + GSTIN + " (" + Mobile + ")"; }
}


/// <summary>
/// Return Result ViewModel for UI to display Return Result details.
/// </summary>
public class ReturnResult
{
    public bool IsSuccess { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public bool IsError { get; set; } = false;
    public string? ErrorMessage { get; set; } = string.Empty;
}