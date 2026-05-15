using Garmetix.Core.Models.Inventory;

namespace Garmetix.Billing.Models
{
    public static class InvoiceItemMappingExtensions
    {
        public static EntryItem? ToEntryItem(this InvoiceItem? item)
        {
            if (item == null) return null;

            // Safely calculate the discount percentage to avoid division by zero
            decimal lineTotalBase = item.BasePrice * item.BilledQuantity;
            decimal calculatedDiscountPercentage = lineTotalBase > 0
                ? (item.DiscountAmount / lineTotalBase) * 100m
                : 0m;

            return new EntryItem
            {
                Id = item.Id,
                InvoiceId = item.InvoiceId,
                Category = item.Category,
                Barcode = item.Barcode,
                BasePrice = item.BasePrice,
                BilledQuantity = item.BilledQuantity,
                DiscountPercentage = calculatedDiscountPercentage,

                // Note: ProductName and Size aren't natively in InvoiceItem. 
                // If you eager-load the Product, you can map them here like:
                // ProductName = item.Product?.Name ?? string.Empty,
                ProductName = string.Empty,
                Size = string.Empty
            };
        }

        public static InvoiceItem? ToInvoiceItem(this EntryItem? dto)
        {
            if (dto == null) return null;

            return new InvoiceItem
            {
                Id = dto.Id,
                InvoiceId = dto.InvoiceId,
                Category = dto.Category,
                Barcode = dto.Barcode,
                BasePrice = dto.BasePrice,
                BilledQuantity = dto.BilledQuantity,
                ActualQuantity = dto.BilledQuantity, // Assuming actual matched billed at entry

                // Re-calculating from the DTO's live calculated fields
                DiscountAmount = dto.DiscountAmount,
                TaxPercentage = dto.GstPercentage,
                TaxAmount = dto.TaxAmount,
                Amount = dto.TotalAmount,

                // MRP is typically BasePrice before tax/discount, or you can map it to a specific DTO field later if added
                MRP = dto.BasePrice,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
