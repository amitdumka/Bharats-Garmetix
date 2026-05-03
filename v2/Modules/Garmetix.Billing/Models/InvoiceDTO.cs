using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Core.Enums; 
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Billing.Models
{

    public partial class InvoiceDTO : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNumber { get; set; }=string.Empty;
        public DateTime OnDate { get; set; } = DateTime.Now;

        public string CustomerName { get; set; } = "Walk-in Customer";
        public string CustomerMobileNumber { get; set; }= string.Empty;
        public string Address { get; set; } = "Dumka";
        public string? CustomerGstin { get; set; } = string.Empty;
        public bool IsInterStateSale { get; set; } = false;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal subTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalDiscount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalTax;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal globalDiscountAmount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal grandTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal paidAmount;
        [ObservableProperty] private decimal roundOffAmount;

        [Ignore] public decimal BalanceAmount => GrandTotal - PaidAmount;
    }
    public partial class InvoiceItemDTO : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; }
        //TODO: create type Garment Type
        public ProductType Category { get; set; }

        [ObservableProperty] private string productName=string.Empty;
        [ObservableProperty] private string barcode=string.Empty;
        [ObservableProperty] private string size=string.Empty;
        [ObservableProperty] private decimal basePrice=0m;

        // CHANGED: Quantity is now a decimal, defaulting to 1m
        [ObservableProperty] private decimal billedQuantity = 1m;

        [ObservableProperty] private decimal discountPercentage=0m;

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
        }
    }
}
