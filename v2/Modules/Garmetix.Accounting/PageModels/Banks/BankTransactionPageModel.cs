using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
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
    public class BankCashTransactionPageModel : PageModel<BankCashTranscation>
    {
        public BankCashTransactionPageModel() : base()
        {
            DefaultSortedColName = nameof(BankCashTranscation.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                 
                new DataGridTextColumn() { HeaderText = nameof(BankCashTranscation.OnDate), MappingName = nameof(BankCashTranscation.OnDate) },
                new DataGridTextColumn() { HeaderText = nameof(BankCashTranscation.TransactionType), MappingName = nameof(BankCashTranscation.TransactionType) },
                new DataGridTextColumn() { HeaderText = nameof(BankCashTranscation.Naration), MappingName = nameof(BankCashTranscation.Naration) },
                new DataGridTextColumn() { HeaderText = nameof(BankCashTranscation.Reference), MappingName = nameof(BankCashTranscation.Reference) },
                new DataGridTextColumn() { HeaderText = nameof(BankCashTranscation.BankAccountId), MappingName = nameof(BankCashTranscation.BankAccountId) },
                new DataGridTextColumn() { HeaderText = nameof(BankCashTranscation.Amount), MappingName = nameof(BankCashTranscation.Amount) },
            ];

            return GridColumns;
        }
    }
}