using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Core.Enums;
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Billing.Models
{
    
    /// <summary>
    /// Invoice DTO 
    /// </summary>
    public partial class InvoiceDTO : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime OnDate { get; set; } = DateTime.Now;

        public string CustomerName { get; set; } = "Walk-in Customer";
        public string CustomerMobileNumber { get; set; } = string.Empty;
        public string Address { get; set; } = "Dumka";
        //public string? CustomerGSTIN { get; set; } = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsB2BSale))]
        private string? _customerGSTIN = string.Empty;
        public bool IsInterStateSale { get; set; } = false;

        // This property will automatically update whenever CustomerGSTIN changes
        public bool IsB2BSale => !string.IsNullOrWhiteSpace(CustomerGSTIN) && CustomerGSTIN.Length == 15;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal subTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalDiscount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal totalTax;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal globalDiscountAmount;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal grandTotal;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(BalanceAmount))] private decimal paidAmount;
        [ObservableProperty] private decimal roundOffAmount;

        // CHANGED: Quantity is now a decimal, defaulting to 1m
        [ObservableProperty] private decimal billedQuantity = 1m;
        [Ignore] public decimal BalanceAmount => GrandTotal - PaidAmount;
    }
}
