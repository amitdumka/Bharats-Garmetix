using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Accounting.PageModels
{
    public class LedgerGroupPageModel : PageModel<LedgerGroup>
    {
        public LedgerGroupPageModel() : base()
        {
            DefaultSortedColName = nameof(LedgerGroup.Name);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(LedgerGroup.Name)),
                AddGridColumns(nameof(LedgerGroup.Category)),

            ];

            return GridColumns;
        }
    }
}