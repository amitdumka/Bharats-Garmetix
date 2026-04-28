using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public class BankAccountPageModel : PageModel<BankAccount>
    {
        public BankAccountPageModel() : base()
        {
            DefaultSortedColName = nameof(BankAccount.AccountNumber);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(BankAccount.AccountNumber)),
                AddGridColumns(nameof(BankAccount.AccountHolderName)),
                AddGridColumns(nameof(BankAccount.AccountType)),
                AddGridColumns(nameof(BankAccount.ClosingBalance)),
                AddGridColumns(nameof(BankAccount.OpeningBalance)),
                AddDateGridColumns(nameof(BankAccount.OpeningDate)),
                AddGridColumns(nameof(BankAccount.Branch)),

                //new DataGridTextColumn() { HeaderText = nameof(BankAccount.AccountNumber), MappingName = nameof(BankAccount.AccountNumber) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccount.AccountHolderName), MappingName = nameof(BankAccount.AccountHolderName) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccount.AccountType), MappingName = nameof(BankAccount.AccountType) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccount.ClosingBalance), MappingName = nameof(BankAccount.ClosingBalance) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccount.OpeningBalance), MappingName = nameof(BankAccount.OpeningBalance) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccount.OpeningDate), MappingName = nameof(BankAccount.OpeningDate) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccount.Branch), MappingName = nameof(BankAccount.Branch) },
            ];

            return GridColumns;
        }
    }
}