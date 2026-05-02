namespace Garmetix.Models.Info;

//ViewModels

/// <summary>
/// VendorInfo ViewModel for UI to display Vendor details. and
/// provides Vendor Information for Get Vendor Call For Entry
/// </summary>
public class VendorInfo
{
    public Guid Id { get; set; }
    public string VendorName { get; set; }
    public string GSTIN { get; set; }
    public string Name { get => VendorName + " (" + GSTIN + ")"; }
}

/// <summary>
/// StockInfo ViewModel for UI to display Stock details.
/// Stock Information for Get Stock Call For Entry
/// </summary>
public class StockInfo
{
    public Guid ProductId { get; set; }
    public string Barcode { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public string? ProductName { get; set; }
    public string? HSNCode { get; set; }
    public bool IsDiscounted { get; set; } = false;
    public string Name { get => ProductName + "-" + Barcode + " (" + HSNCode + ")"; }
}

/// <summary>
/// CustomerInfo ViewModel for UI to display Customer details.
/// </summary>
public class CustomerInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Mobile { get; set; }
    public string? GSTIN { get; set; }
    public int Count { get; set; } = 0;
    public decimal PurchaseValue { get; set; } = 0;
    public decimal LoyaltyPoints { get; set; } = 0;
    public string CustomerName { get => Name + " - " + GSTIN + " (" + Mobile + ")"; }
}


/// <summary>
/// Return Result ViewModel for UI to display Return Result details.
/// </summary>
public class ReturnResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
}