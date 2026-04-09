using System;
using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Garmetix.AI.Billing.Models
{
    public enum GarmentCategory { Fabric, ReadyMade, Accessories }

    // --- NEW: Customer Model for Database lookups ---
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string Gstin { get; set; }
    }

    // --- NEW: Payment Detail Model for Split Payments ---
    public class PaymentDetail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid InvoiceId { get; set; } // Foreign key to link to Invoice
        public string Mode { get; set; }    // Cash, UPI, Card, etc.
        public decimal Amount { get; set; }
    }

    public class Product
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Barcode { get; set; }
        public GarmentCategory Category { get; set; }
        public string HsnCode { get; set; }
        public decimal BaseRate { get; set; }
        public string DefaultSize { get; set; }
        public string Color { get; set; }
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

        // Converted to ObservableProperties so UI updates automatically upon calculation
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BalanceAmount))]
        private decimal subTotal;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BalanceAmount))]
        private decimal totalDiscount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BalanceAmount))]
        private decimal totalTax;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BalanceAmount))]
        private decimal grandTotal;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BalanceAmount))]
        private decimal paidAmount;

        // ADD THIS NEW PROPERTY
        [ObservableProperty]
        private decimal roundOffAmount;
        [Ignore] // Tells SQLite not to attempt to save this calculated property to the DB
        public decimal BalanceAmount => GrandTotal - PaidAmount;
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
        [ObservableProperty] private int quantity = 1;
        [ObservableProperty] private decimal discountAmount;

        [Ignore]
        public decimal GstPercentage
        {
            get
            {
                if (Category == GarmentCategory.Fabric) return 5m;

                decimal unitDiscount = Quantity > 0 ? DiscountAmount / Quantity : 0;
                decimal unitTaxableValue = Rate - unitDiscount;
                return unitTaxableValue > 2499 ? 18m : 5m;
            }
        }

        [Ignore]
        public decimal TaxableValue => (Rate * Quantity) - DiscountAmount;

        [Ignore]
        public decimal TaxAmount => TaxableValue * (GstPercentage / 100m);

        [Ignore]
        public decimal TotalAmount => TaxableValue + TaxAmount;

        partial void OnRateChanged(decimal value) => Refresh();
        partial void OnQuantityChanged(int value) => Refresh();
        partial void OnDiscountAmountChanged(decimal value) => Refresh();

        private void Refresh()
        {
            OnPropertyChanged(nameof(GstPercentage));
            OnPropertyChanged(nameof(TaxableValue));
            OnPropertyChanged(nameof(TaxAmount));
            OnPropertyChanged(nameof(TotalAmount));
        }
    }
}