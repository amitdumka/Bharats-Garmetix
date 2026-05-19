using Garmetix.Core.Models.Inventory;

namespace Garmetix.Billing.Models
{
    public static class InvoiceMappingExtensions
    {
        public static InvoiceDTO? ToInvoiceDto(this Invoice? inv)
        {
            if (inv == null) return null;

            return new InvoiceDTO
            {
                Id = inv.Id,
                InvoiceNumber = inv.InvoiceNumber ?? string.Empty,
                OnDate = inv.OnDate,

                // Retain default "Walk-in Customer" if the domain model has no name
                CustomerName = string.IsNullOrWhiteSpace(inv.CustomerName) ? "Walk-in Customer" : inv.CustomerName,
                CustomerMobileNumber = inv.CustomerMobileNumber ?? string.Empty,
                CustomerGSTIN = inv.CustomerGSTIN,

                IsInterStateSale = inv.InterState,
                // Note: IsB2BSale is read-only in DTO and automatically calculated based on CustomerGSTIN length

                // Financials
                SubTotal = inv.NetAmount,             // BaseInvoice labels NetAmount as "Net Amount/Sub Total"
                TotalDiscount = inv.DiscountAmount,
                GlobalDiscountAmount = inv.BillDiscountAmount,
                TotalTax = inv.TaxAmount,
                GrandTotal = inv.BillAmount,
                PaidAmount = inv.PaidAmount,
                RoundOffAmount = inv.RoundOff,

                // Quantities
                BilledQuantity = inv.Quantity, 
                Address= "" // Address is not present in the domain model, so we set it to an empty string or could be mapped from another source if needed
                
                

            };
        }

        [Obsolete("This method is deprecated. Use ToInvoice(InvoiceDTO dto) instead for better handling of nullability and default values.")]
        public static Invoice? ToInvoice(this InvoiceDTO? dto)
        {
            if (dto == null) return null;

            return new Invoice
            {
                // BaseEntity / Base properties
                Id = dto.Id,
                UpdatedAt = DateTime.UtcNow,

                // BaseInvoice properties
                InvoiceNumber = dto.InvoiceNumber,
                OnDate = dto.OnDate,
                InterState = dto.IsInterStateSale,

                NetAmount = dto.SubTotal,
                DiscountAmount = dto.TotalDiscount,
                TaxAmount = dto.TotalTax,
                BillAmount = dto.GrandTotal,
                RoundOff = dto.RoundOffAmount,

                //BilledQuantity = dto.BilledQuantity,
                Quantity = dto.BilledQuantity, // Assuming base quantity matches billed quantity on DTO conversion
                //ActualQuantity= dto.BilledQuantity, // Assuming actual quantity matches billed quantity on DTO conversion

                // Invoice specific properties
                CustomerName = dto.CustomerName,
                CustomerMobileNumber = dto.CustomerMobileNumber,
                CustomerGSTIN = dto.CustomerGSTIN,
                B2BSale = dto.IsB2BSale,
                BillDiscountAmount = dto.GlobalDiscountAmount,
                PaidAmount = dto.PaidAmount, 
                Deleted = false // New invoices created from DTO are not deleted
               , ItemCount=0, Synced=false, CreatedAt= DateTime.UtcNow, ReturnInvoice=false
            };
        }
    }
}
