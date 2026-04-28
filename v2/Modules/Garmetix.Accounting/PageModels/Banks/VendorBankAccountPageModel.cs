using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public class VendorBankAccountPageModel : PageModel<VendorBankAccount>
    {
        public VendorBankAccountPageModel() : base()
        {
            DefaultSortedColName = nameof(VendorBankAccount.AccountHolderName);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(VendorBankAccount.AccountNumber)),
                AddGridColumns(nameof(VendorBankAccount.AccountHolderName)),
                AddGridColumns(nameof(VendorBankAccount.AccountType)),
                AddGridColumns(nameof(VendorBankAccount.ClosingBalance)),
                AddGridColumns(nameof(VendorBankAccount.OpeningBalance)),
                AddDateGridColumns(nameof(VendorBankAccount.OpeningDate)),
                AddGridColumns(nameof(VendorBankAccount.Branch)),

                //new DataGridTextColumn() { HeaderText = nameof(VendorBankAccount.AccountNumber), MappingName = nameof(VendorBankAccount.AccountNumber) },
                //new DataGridTextColumn() { HeaderText = nameof(VendorBankAccount.AccountHolderName), MappingName = nameof(VendorBankAccount.AccountHolderName) },
                //new DataGridTextColumn() { HeaderText = nameof(VendorBankAccount.AccountType), MappingName = nameof(VendorBankAccount.AccountType) },
                //new DataGridTextColumn() { HeaderText = nameof(VendorBankAccount.ClosingBalance), MappingName = nameof(VendorBankAccount.ClosingBalance) },
                //new DataGridTextColumn() { HeaderText = nameof(VendorBankAccount.OpeningBalance), MappingName = nameof(VendorBankAccount.OpeningBalance) },
                //new DataGridTextColumn() { HeaderText = nameof(VendorBankAccount.OpeningDate), MappingName = nameof(VendorBankAccount.OpeningDate) },
                //new DataGridTextColumn() { HeaderText = nameof(VendorBankAccount.Branch), MappingName = nameof(VendorBankAccount.Branch) },
            ];

            return GridColumns;
        }
    }
}