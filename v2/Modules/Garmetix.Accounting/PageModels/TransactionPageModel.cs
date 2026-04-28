using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Accounting.PageModels
{

    public class TransactionPageModel : PageModel<Transaction>
    {
        public TransactionPageModel() : base()
        {
            DefaultSortedColName = nameof(Transaction.Name);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                new DataGridTextColumn() { HeaderText = nameof(Transaction.Name), MappingName = nameof(Transaction.Name) },
            ];
            return GridColumns;
        }
    }
}