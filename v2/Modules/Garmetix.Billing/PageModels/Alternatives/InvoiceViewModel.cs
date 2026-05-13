using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Core.Models.Accounting;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.ViewModels;




// Single invoice line item
internal class InvoiceItem
{
    public string ItemCode { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal DiscountPercent { get; set; } // New property for discount percentage
    public decimal Discount => Quantity * Rate * DiscountPercent / 100m; // Calculate discount amount
    public decimal Amount => Quantity * Rate - Discount;
    public decimal GstPercent { get; set; } = 12m; // default 12%
    public decimal Cgst => Amount * GstPercent / 200m;
    public decimal Sgst => Amount * GstPercent / 200m;
    public decimal Total => Amount + Cgst + Sgst;
}
