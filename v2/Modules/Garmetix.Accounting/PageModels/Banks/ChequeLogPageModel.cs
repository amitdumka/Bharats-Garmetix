using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Accounting.PageModels
{
    public class ChequeLogPageModel : PageModel<ChequeLog>
    {
        public ChequeLogPageModel() : base()
        {
            DefaultSortedColName = nameof(ChequeLog.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                new DataGridTextColumn() { HeaderText = nameof(ChequeLog.OnDate), MappingName = nameof(ChequeLog.OnDate) },
                new DataGridTextColumn() { HeaderText = nameof(ChequeLog.Narration), MappingName = nameof(ChequeLog.Narration) },
                new DataGridTextColumn() { HeaderText = nameof(ChequeLog.ChequeNumber), MappingName = nameof(ChequeLog.ChequeNumber) },
                new DataGridTextColumn() { HeaderText = nameof(ChequeLog.ChequeDate), MappingName = nameof(ChequeLog.ChequeDate) },
                new DataGridTextColumn() { HeaderText = nameof(ChequeLog.BankAccountId), MappingName = nameof(ChequeLog.BankAccountId) },
                new DataGridTextColumn() { HeaderText = nameof(ChequeLog.InHouse), MappingName = nameof(ChequeLog.InHouse) },
                new DataGridTextColumn() { HeaderText = nameof(ChequeLog.Amount), MappingName = nameof(ChequeLog.Amount) },
            ];

            return GridColumns;
        }
    }
}