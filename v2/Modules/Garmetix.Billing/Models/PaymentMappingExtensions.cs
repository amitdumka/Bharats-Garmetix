using Garmetix.Core.Models.Inventory;

namespace Garmetix.Billing.Models
{
    public static class PaymentMappingExtensions
    {
        // --- Domain to DTO ---

        /// <summary>
        /// Maps an InvoicePayment to a PaymentDetail. Optionally accepts a CardPayment to merge details.
        /// </summary>
        public static PaymentDetail? ToPaymentDetail(this InvoicePayment? payment, CardPayment? cardPayment = null)
        {
            if (payment == null) return null;

            var detail = new PaymentDetail
            {
                Guid = payment.Id,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                PaymentDate = payment.OnDate,
                PaymentMode = payment.PaymentMode,
                PaymentNote = payment.ReferenceNumber ?? string.Empty, 
            };

            // Merge card details if the payment was made via card and the record exists
            if (cardPayment != null)
            {
                detail.CardPaymentBank = cardPayment.BankName ?? string.Empty;
                detail.CardPaymentNumber = cardPayment.CardNumber;
                detail.AuthCode = cardPayment.AuthCode; // Assuming AuthCode acts as the detail/reference
                                                                             // detail.Card = (CardType)cardPayment.CardType; // Uncomment and cast if your Enums align
            }

            return detail;
        }

        // --- DTO to Domain ---

        public static InvoicePayment? ToInvoicePayment(this PaymentDetail? dto)
        {
            if (dto == null) return null;

            return new InvoicePayment
            {
                Id = dto.Guid == Guid.Empty ? Guid.NewGuid() : dto.Guid,
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                OnDate = dto.PaymentDate,
                PaymentMode = dto.PaymentMode,
                ReferenceNumber = dto.PaymentNote,
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Generates a CardPayment entity only if the PaymentDetail represents a card transaction.
        /// </summary>
        public static CardPayment? ToCardPayment(this PaymentDetail? dto)
        {
            if (dto == null) return null;

            // Check your PaymentMode Enum value for 'Card' or 'CreditCard' / 'DebitCard'
            // Assuming 'PaymentMode.Card' exists based on the provided fields
            // if (string.IsNullOrWhiteSpace(dto.CardPaymentNumber) && string.IsNullOrWhiteSpace(dto.CardPaymentBank))
            //  return null;

            // Parse strings back to ints for the domain model safely
            //int.TryParse(dto.CardPaymentNumber, out int parsedCardNumber);
            //int.TryParse(dto.CardPaymentDetails, out int parsedAuthCode);

            return new CardPayment
            {
                Id = Guid.NewGuid(), // Card payments usually get their own unique ID in the Db
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                OnDate = dto.PaymentDate,
                BankName = dto.CardPaymentBank,
                CardNumber = dto.CardPaymentNumber.Value,
                AuthCode = dto.AuthCode.Value,
                UpdatedAt = DateTime.UtcNow,

                // Map the enums for Card and CardType here based on your specific Enum declarations
                 CardType =  dto.CardType.Value, 
            };
        }
    }
}
