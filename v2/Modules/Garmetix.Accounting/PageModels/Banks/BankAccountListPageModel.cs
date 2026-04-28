using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public class BankAccountListPageModel : PageModel<BankAccountList>
    {
        public BankAccountListPageModel() : base()
        {
            DefaultSortedColName = nameof(BankAccountList.AccountHolderName);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(BankAccountList.AccountNumber)),
                AddGridColumns(nameof(BankAccountList.AccountHolderName)),
                AddGridColumns(nameof(BankAccountList.AccountType)),
                AddGridColumns(nameof(BankAccountList.BankName)),
                AddGridColumns(nameof(BankAccountList.Branch)),
                AddDateGridColumns(nameof(BankAccountList.IFSCode)),
                GetEditDeleteButtons(),

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