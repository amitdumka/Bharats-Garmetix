using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Billing.Models
{
    public class PaymentDetail
    {
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; } = Guid.Empty;
        public string? InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; } = decimal.Zero;
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        public string? PaymentNote { get; set; } = string.Empty;
        public int? AuthCode { get; set; } = null;
        public int? CardPaymentNumber { get; set; } = null;
        public string? CardPaymentBank { get; set; } = string.Empty;
        public Card? Card { get; set; } = Garmetix.Core.Enums.Card.DebitCard;
        public CardType? CardType { get; set; } = Garmetix.Core.Enums.CardType.Rupay;
    }

    internal class BillingSettings
    {
        [Key]
        public Guid Guid { get; set; } = Guid.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string? CompanyPhone { get; set; } = string.Empty;
        public string? CompanyEmail { get; set; } = string.Empty;
        public string? CompanyGSTIN { get; set; } = string.Empty;
        public string? CompanyState { get; set; } = string.Empty;
    }

    public partial class EntryItem : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; }
        public ProductType Category { get; set; }

        [ObservableProperty] private string productName = string.Empty;
        [ObservableProperty] private string barcode = string.Empty;
        [ObservableProperty] private string size = string.Empty;
        [ObservableProperty] private decimal basePrice = 0m;
        [ObservableProperty] private decimal billedQuantity = 1m;
        [ObservableProperty] private decimal discountPercentage = 0m;

        [NotMapped]
        public decimal DiscountAmount => Math.Round((BasePrice * BilledQuantity) * (DiscountPercentage / 100m), 2);

        [NotMapped]
        public decimal GstPercentage
        {
            get
            {
                if (Category == ProductType.Fabric) return 5m;
                decimal unitDiscount = BilledQuantity > 0 ? DiscountAmount / BilledQuantity : 0;
                decimal unitTaxableValue = BasePrice - unitDiscount;
                return unitTaxableValue > 2499 ? 18m : 5m;
            }
        }

        [NotMapped] public decimal TaxableValue => (BasePrice * BilledQuantity) - DiscountAmount;
        [NotMapped] public decimal TaxAmount => Math.Round(TaxableValue * (GstPercentage / 100m), 2);
        [NotMapped] public decimal TotalAmount => TaxableValue + TaxAmount;

        partial void OnBasePriceChanged(decimal value) => Refresh();
        partial void OnBilledQuantityChanged(decimal value) => Refresh();
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

    public partial class InvoiceDTO : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime OnDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = "Walk-in Customer";
        public string CustomerMobileNumber { get; set; } = string.Empty;
        public string Address { get; set; } = "Dumka";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsB2BSale))]
        private string? _customerGSTIN = string.Empty;

        public bool IsInterStateSale { get; set; } = false;

        [NotMapped]
        public bool IsB2BSale => !string.IsNullOrWhiteSpace(CustomerGSTIN) && CustomerGSTIN.Length == 15;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal subTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalDiscount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalTax;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal globalDiscountAmount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal grandTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal paidAmount;
        [ObservableProperty] private decimal roundOffAmount;
        [ObservableProperty] private decimal billedQuantity = 1m;

        [NotMapped]
        public decimal BalanceAmount => GrandTotal - PaidAmount;
    }
}