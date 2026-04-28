using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels.Vouchers
{
    public class CashVoucherPageModel : PageModel<CashVoucher> 
        {
        public CashVoucherPageModel() : base()
        {
            DefaultSortedColName = nameof(CashVoucher.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(CashVoucher.VoucherType)),
                new DataGridTextColumn() { HeaderText = "Transcation", MappingName ="Transaction.Name" },
                AddGridColumns(nameof(CashVoucher.VoucherNumber)),
                AddGridColumns(nameof(CashVoucher.PartyName)),
                AddGridColumns(nameof(CashVoucher.Particulars)),
                AddGridColumns(nameof(CashVoucher.Amount)),
                AddDateGridColumns(nameof(CashVoucher.OnDate)),
                new DataGridTextColumn() { HeaderText = "Ledger", MappingName = "Ledger.Name" },
                AddGridColumns(nameof(CashVoucher.Remarks)),
                new DataGridTextColumn() { HeaderText = "Issued By", MappingName = "Employee.FullName" },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.VoucherNumber), MappingName = nameof(CashVoucher.VoucherNumber) },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.PartyName), MappingName = nameof(CashVoucher.PartyName) },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.Particulars), MappingName = nameof(CashVoucher.Particulars) },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.Amount), MappingName = nameof(CashVoucher.Amount) },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.OnDate), MappingName = nameof(CashVoucher.OnDate), Format = "dd/MMM/yyyy" },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.TransactionId), MappingName = nameof(CashVoucher.TransactionId) },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.LedgerId), MappingName = nameof(CashVoucher.LedgerId) },
                //new DataGridTextColumn() { HeaderText = nameof(CashVoucher.Remarks), MappingName = nameof(CashVoucher.Remarks) },
            ];

            return GridColumns;
        }
    }
}