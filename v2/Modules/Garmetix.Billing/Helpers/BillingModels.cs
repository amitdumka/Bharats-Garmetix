using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Text.Json.Serialization;

namespace OldSystem.Models.NeedToRemoved
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

    
    
    
}