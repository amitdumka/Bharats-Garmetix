using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public class DueRecoveryPageModel : PageModel<DueRecovery>
    {
        public DueRecoveryPageModel() : base()
        {
            DefaultSortedColName = nameof(DueRecovery.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns = [
                AddGridColumns(nameof(DueRecovery.InvoiceNumber)),
                AddGridColumns(nameof(DueRecovery.Amount)),
                AddGridColumns(nameof(DueRecovery.Paid)),
                AddDateGridColumns(nameof(DueRecovery.OnDate)),
                AddGridColumns(nameof(DueRecovery.PaymentMode)),
                AddGridColumns(nameof(DueRecovery.PaymentDetails)),

 ];
            return GridColumns;
        }
    }
}