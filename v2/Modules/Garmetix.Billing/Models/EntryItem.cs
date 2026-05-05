using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Core.Enums;
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Billing.Models
{
    /// <summary>
    /// Entry Item is used to add item into cart , so it can be used to Enter in invoice 
    /// </summary>
    public partial class EntryItem : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; }
        //TODO: create type Garment Type
        public ProductType Category { get; set; }

        [ObservableProperty] private string productName = string.Empty;
        [ObservableProperty] private string barcode = string.Empty;
        [ObservableProperty] private string size = string.Empty;
        [ObservableProperty] private decimal basePrice = 0m;

        // CHANGED: Quantity is now a decimal, defaulting to 1m
        [ObservableProperty] private decimal billedQuantity = 1m;

        [ObservableProperty] private decimal discountPercentage = 0m;

        [Ignore]
        public decimal DiscountAmount => (BasePrice * BilledQuantity) * (DiscountPercentage / 100m);

        [Ignore]
        public decimal GstPercentage
        {
            get
            {
                if (Category == ProductType.Fabric) return 5m;
                // Decimal division is now perfectly safe here
                decimal unitDiscount = BilledQuantity > 0 ? DiscountAmount / BilledQuantity : 0;
                decimal unitTaxableValue = BasePrice - unitDiscount;
                return unitTaxableValue > 2499 ? 18m : 5m;
            }
        }

        [Ignore] public decimal TaxableValue => (BasePrice * BilledQuantity) - DiscountAmount;
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
            OnPropertyChanged(nameof(BilledQuantity));
            
        }
    }
}
