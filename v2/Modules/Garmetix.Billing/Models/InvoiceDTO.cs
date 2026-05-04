using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Core.Enums;
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Billing.Models
{

    /// <summary>
    /// Billing Setting Class is used to set the biller information.
    /// so it can be used to supply Biller details to Invoice pdf or print
    /// </summary>
    internal class BillingSettings
    {
        public Guid Guid { get; set; } = Guid.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string? CompanyPhone { get; set; } = string.Empty;
        public string? CompanyEmail { get; set; } = string.Empty;
        public string? CompanyGSTIN { get; set; } = string.Empty;
        public string? CompanyState { get; set; } = string.Empty;
    }
    /// <summary>
    /// Payment Details is used record the payment in one go so later it can set the payment details based on the type.
    /// </summary>
    internal class PaymentDetail
    {
        [PrimaryKey, AutoIncrement]
        public Guid Guid { get; set; } = Guid.Empty;
        public Guid InvoiceId { get; set; } = Guid.Empty;
        public string? InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; } = decimal.Zero;
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        public string? PaymentNote { get; set; } = string.Empty;

        public string? CardPaymentDetails { get; set; } = string.Empty;
        public string? CardPaymentNumber { get; set; } = string.Empty;
        public string? CardPaymentBank { get; set; } = string.Empty;
        public CardType? Card { get; set; } = CardType.Debit;
    }
     
    public partial class InvoiceDTO : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime OnDate { get; set; } = DateTime.Now;

        public string CustomerName { get; set; } = "Walk-in Customer";
        public string CustomerMobileNumber { get; set; } = string.Empty;
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
        }
    }
}
