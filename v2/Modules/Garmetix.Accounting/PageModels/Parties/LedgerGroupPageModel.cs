using Garmetix.Base.PageModels;
using Garmetix.Core.Models.Accounting;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels.Parties
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