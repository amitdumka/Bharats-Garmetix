using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Accounting.PageModels
{
    public class BankTransactionPageModel : PageModel<BankTransaction>
    {
        public BankTransactionPageModel() : base()
        {
            DefaultSortedColName = nameof(BankTransaction.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                //GridColumns.Add(new DataGridTextColumn() { HeaderText = nameof(Bank.Id), MappingName = nameof(Bank.Id) });
                new DataGridTextColumn() { HeaderText = nameof(BankTransaction.OnDate), MappingName = nameof(BankTransaction.OnDate) },
                new DataGridTextColumn() { HeaderText = nameof(BankTransaction.TransactionType), MappingName = nameof(BankTransaction.TransactionType) },
                new DataGridTextColumn() { HeaderText = nameof(BankTransaction.TransactionMode), MappingName = nameof(BankTransaction.TransactionMode) },
                new DataGridTextColumn() { HeaderText = nameof(BankTransaction.Narration), MappingName = nameof(BankTransaction.Narration) },
                new DataGridTextColumn() { HeaderText = nameof(BankTransaction.Reference), MappingName = nameof(BankTransaction.Reference) },
                new DataGridTextColumn() { HeaderText = nameof(BankTransaction.BankAccountId), MappingName = nameof(BankTransaction.BankAccountId) },
                new DataGridTextColumn() { HeaderText = nameof(BankTransaction.Amount), MappingName = nameof(BankTransaction.Amount) },
            ];

            return GridColumns;
        }
    }
}