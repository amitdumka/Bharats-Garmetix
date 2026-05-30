using CommunityToolkit.Mvvm.ComponentModel;
namespace Garmetix.Billing.PageModels
{
    // UI-Specific wrapper to handle selection and return quantity tracking
    public partial class ReturnItemWrapper : ObservableObject
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public decimal Rate { get; set; }
        public decimal OriginalQty { get; set; }

        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal LineTotal { get; set; } = 0;

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private decimal _returnQuantity;

        [ObservableProperty]
        private decimal _totalRefund;

        // Auto-calculate totals and prevent returning more than purchased
        partial void OnReturnQuantityChanged(decimal value)
        {
            if (value > OriginalQty)
            {
                ReturnQuantity = OriginalQty; // Cap at max purchased
            }
            TotalRefund = ReturnQuantity * Rate;
        }
    }
}