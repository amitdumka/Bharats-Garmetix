using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Text.Json.Serialization;

namespace Garmetix.AI.Billing.Models
{
    public enum GarmentCategory { Fabric, ReadyMade, Accessories }
    public class Stock
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; } // Link to master Product table

        [Indexed] // Indexed for hyper-fast lookups during billing
        public string Barcode { get; set; }

        public GarmentCategory Category { get; set; }

        public decimal PurchasedQty { get; set; }
        public decimal SoldQty { get; set; }
        [JsonIgnore]
        public decimal CurrentQty => PurchasedQty - SoldQty;

        public decimal BasicCostRate { get; set; }
        public decimal BasicSaleRate { get; set; }
        public decimal TaxRate { get; set; }
    }
    public class PurchaseInvoice
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Internal Tracking
        public string InwardNo { get; set; }
        public DateTime InwardDate { get; set; } = DateTime.Now;

        // Vendor Details
        public string VendorInvoiceNo { get; set; }
        public DateTime VendorInvoiceDate { get; set; } = DateTime.Now;
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorGstin { get; set; }

        // Totals
        public decimal SubTotal { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public string Notes { get; set; }
    }

    public partial class PurchaseItem : ObservableObject
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PurchaseInvoiceId { get; set; }

        // NEW: This is required to link the item to your Stock table!
        public string Barcode { get; set; }

          
        

        [ObservableProperty] private string productName;
        [ObservableProperty] private decimal rate;
        [ObservableProperty] private decimal quantity = 1m;
        [ObservableProperty] private decimal taxPercentage;

        [Ignore] public decimal TaxableValue => Rate * Quantity;
        [Ignore] public decimal TaxAmount => TaxableValue * (TaxPercentage / 100m);
        [Ignore] public decimal TotalAmount => TaxableValue + TaxAmount;

        partial void OnRateChanged(decimal value) => Refresh();
        partial void OnQuantityChanged(decimal value) => Refresh();
        partial void OnTaxPercentageChanged(decimal value) => Refresh();

        private void Refresh()
        {
            OnPropertyChanged(nameof(TaxableValue));
            OnPropertyChanged(nameof(TaxAmount));
            OnPropertyChanged(nameof(TotalAmount));
        }
    }
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string Gstin { get; set; }
    }

    public class PaymentDetail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string Mode { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }= DateTime.Now;
    }

    // --- MERGED & UPGRADED PRODUCT CLASS ---
    public class Product
    {
        [PrimaryKey]
        public string Barcode { get; set; }
        public string Name { get; set; }
        public GarmentCategory Category { get; set; }
        public string HsnCode { get; set; }
        public decimal BaseRate { get; set; }
        public string DefaultSize { get; set; }
        public string Color { get; set; }
        public decimal CurrentQty { get; set; }
        public string Unit { get; set; }
        public decimal TaxRate { get; set; }

        // NEW: Combines Barcode and Name for the UI Search Box
        [Ignore]
        public string DisplayText => $"{Barcode} - {Name}";
    }

    public partial class Invoice : ObservableObject
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Gstin { get; set; }
        public bool IsInterStateSale { get; set; }

        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal subTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalDiscount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalTax;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal globalDiscountAmount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal grandTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal paidAmount;
        [ObservableProperty] private decimal roundOffAmount;

        [Ignore] public decimal BalanceAmount => GrandTotal - PaidAmount;
    }
    public partial class InvoiceItem : ObservableObject
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; }
        public GarmentCategory Category { get; set; }

        [ObservableProperty] private string productName;
        [ObservableProperty] private string size;
        [ObservableProperty] private decimal rate;

        // CHANGED: Quantity is now a decimal, defaulting to 1m
        [ObservableProperty] private decimal quantity = 1m;

        [ObservableProperty] private decimal discountPercentage;

        [Ignore]
        public decimal DiscountAmount => (Rate * Quantity) * (DiscountPercentage / 100m);

        [Ignore]
        public decimal GstPercentage
        {
            get
            {
                if (Category == GarmentCategory.Fabric) return 5m;
                // Decimal division is now perfectly safe here
                decimal unitDiscount = Quantity > 0 ? DiscountAmount / Quantity : 0;
                decimal unitTaxableValue = Rate - unitDiscount;
                return unitTaxableValue > 2499 ? 18m : 5m;
            }
        }

        [Ignore] public decimal TaxableValue => (Rate * Quantity) - DiscountAmount;
        [Ignore] public decimal TaxAmount => TaxableValue * (GstPercentage / 100m);
        [Ignore] public decimal TotalAmount => TaxableValue + TaxAmount;

        partial void OnRateChanged(decimal value) => Refresh();

        // CHANGED: This must now accept a decimal instead of an int
        partial void OnQuantityChanged(decimal value) => Refresh();

        partial void OnDiscountPercentageChanged(decimal value) => Refresh();

        private void Refresh()
        {
            OnPropertyChanged(nameof(DiscountAmount));
            OnPropertyChanged(nameof(GstPercentage));
            OnPropertyChanged(nameof(TaxableValue));
            OnPropertyChanged(nameof(TaxAmount));
            OnPropertyChanged(nameof(TotalAmount));
        }
    }
    
}