
using Garmetix.Core.Enums;
using SQLite;

namespace Garmetix.Billing.Models
{
    /// <summary>
    /// Payment Details is used record the payment in one go so later it can set the payment details based on the type.
    /// </summary>
    public class PaymentDetail
    {
        [PrimaryKey, AutoIncrement]
        public Guid Guid { get; set; } = Guid.Empty;
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
        public CardType? CardType { get; set; } = CardType.Rupay;
    }
}
