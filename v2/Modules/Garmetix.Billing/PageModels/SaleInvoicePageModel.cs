using Garmetix.Base.PageModels;
using Garmetix.Core.Models.Inventory;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Billing.PageModels
{
    // A lightweight DTO specifically shaped for the UI DataGrid

    internal partial class SaleInvoicePageModel : PageModel<Invoice>
    {

        public SaleInvoicePageModel() : base()
        {
            DefaultSortedColName = nameof(Invoice.OnDate);
            DefaultSortedOrder = Descending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns = [

                AddDateGridColumns(nameof(Invoice.OnDate)),
                AddGridColumns(nameof(Invoice.InvoiceNumber)),
                AddGridColumns(nameof(Invoice.CustomerName)),
                AddGridColumns(nameof(Invoice.CustomerGSTIN)),
                AddGridColumns(nameof(Invoice.CustomerMobileNumber)),

                AddGridColumns(nameof(Invoice.BasePrice)),
                AddGridColumns(nameof(Invoice.TaxAmount)),
                AddGridColumns(nameof(Invoice.DiscountAmount)),
                AddGridColumns(nameof(Invoice.BillAmount)),
                AddGridColumns(nameof(Invoice.BilledQuantity)),
                AddGridColumns(nameof(Invoice.PaymentMode)),
                AddGridColumns(nameof(Invoice.PaidAmount)),
                AddGridColumns(nameof(Invoice.BalanceAmount)),



                   ];
            return GridColumns;
        }
    }
}