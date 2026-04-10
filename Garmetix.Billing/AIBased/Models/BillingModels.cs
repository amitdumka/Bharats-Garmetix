using System;
using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Garmetix.AI.Billing.Models
{
    public enum GarmentCategory { Fabric, ReadyMade, Accessories }

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