using Garmetix.Core.PageModels;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public partial class VoucherPageModel : PageModel<Voucher>
    {
        public VoucherPageModel() : base()
        {
            DefaultSortedColName = nameof(Voucher.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [

                AddGridColumns(nameof(Voucher.VoucherType)),
                AddGridColumns(nameof(Voucher.VoucherNumber)),
                AddGridColumns(nameof(Voucher.PartyName)),
                AddGridColumns(nameof(Voucher.Particulars)),
                AddGridColumns(nameof(Voucher.Amount)),
                AddGridColumns(nameof(Voucher.PaymentMode)),
                AddGridColumns(nameof(Voucher.PaymentDetails)),
                AddGridColumns(nameof(Voucher.Remarks)),
                AddDateGridColumns(nameof(Voucher.OnDate)),
                new DataGridTextColumn() { HeaderText = "Ledger", MappingName = "Ledger.Name" },
                new DataGridTextColumn() { HeaderText = "Bank Account", MappingName = "BankAccount.AccountHolderName" },
                new DataGridTextColumn() { HeaderText = "Issued By", MappingName = "Employee.FullName" },

            ];

            return GridColumns;
        }
    }
}