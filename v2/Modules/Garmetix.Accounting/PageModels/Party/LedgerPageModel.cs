using Garmetix.Base.PageModels;
using Garmetix.Core.Models.Accounting;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public class LedgerPageModel : PageModel<Ledger>
    {
        public LedgerPageModel() : base()
        {
            DefaultSortedColName = nameof(Ledger.Name);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(Ledger.Name)),
                AddGridColumns(nameof(Ledger.LedgerType)),
                AddGridColumns(nameof(Ledger.LedgerGroupId)),
                AddDateGridColumns(nameof(Ledger.OpeningDate)),
                AddGridColumns(nameof(Ledger.OpeningBalance)),

                //new DataGridTextColumn() { HeaderText = nameof(Ledger.LedgerType), MappingName = nameof(Ledger.LedgerType) },
                //new DataGridTextColumn() { HeaderText = nameof(Ledger.Name), MappingName = nameof(Ledger.Name) },
                //new DataGridTextColumn() { HeaderText = nameof(Ledger.LedgerGroupId), MappingName = nameof(Ledger.LedgerGroup) },
                //new DataGridTextColumn() { HeaderText = nameof(Ledger.OpenningDate), MappingName = nameof(Ledger.OpenningDate), Format = "dd/MMM/yyyy" },
                //new DataGridTextColumn() { HeaderText = nameof(Ledger.OpenningBalance), MappingName = nameof(Ledger.OpenningBalance) },
            ];
            return GridColumns;
        }
    }
}