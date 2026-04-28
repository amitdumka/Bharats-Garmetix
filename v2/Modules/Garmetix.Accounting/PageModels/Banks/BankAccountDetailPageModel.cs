using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Accounting.PageModels
{
    public class BankAccountDetailPageModel : PageModel<BankAccountDetail>
    {
        public BankAccountDetailPageModel() : base()
        {
            DefaultSortedColName = nameof(BankAccountDetail.BankAccountId);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(BankAccountDetail.BankAccountId)),
                AddGridColumns(nameof(BankAccountDetail.ATMCard)),
                AddGridColumns(nameof(BankAccountDetail.ATMPin)),
                AddGridColumns(nameof(BankAccountDetail.EPIN)),
                AddGridColumns(nameof(BankAccountDetail.MPin)),
                AddGridColumns(nameof(BankAccountDetail.CVV)),
                AddDateGridColumns(nameof(BankAccountDetail.ExpireDate)),
                AddGridColumns(nameof(BankAccountDetail.UserName)),
                AddGridColumns(nameof(BankAccountDetail.Password)),

                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.ATMCard), MappingName = nameof(BankAccountDetail.ATMCard) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.ATMPin), MappingName = nameof(BankAccountDetail.ATMPin) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.EPIN), MappingName = nameof(BankAccountDetail.EPIN) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.MPin), MappingName = nameof(BankAccountDetail.MPin) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.CVV), MappingName = nameof(BankAccountDetail.CVV) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.ExpireDate), MappingName = nameof(BankAccountDetail.ExpireDate) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.UserName), MappingName = nameof(BankAccountDetail.UserName) },
                //new DataGridTextColumn() { HeaderText = nameof(BankAccountDetail.Password), MappingName = nameof(BankAccountDetail.Password) },
            ];

            return GridColumns;
        }
    }
}