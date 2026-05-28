using Garmetix.Base.PageModels;
using Garmetix.Models.DayOperations;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Commons.DayOperations.PageModels
{
    public class PettyCashSheetPageModel : PageModel<PettyCashSheet>
    {
        public PettyCashSheetPageModel()
        {
            DefaultSortedColName = nameof(PettyCashSheet.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
           [
                 GetColumn(nameof(PettyCashSheet.OnDate)),
                 GetColumn(nameof(PettyCashSheet.OpeningBalance)),
                 GetColumn(nameof(PettyCashSheet.CashInHand)),
                 GetColumn(nameof(PettyCashSheet.Sales)),
                 GetColumn(nameof(PettyCashSheet.Receipts)),
                 GetColumn(nameof(PettyCashSheet.DueReceipts)),
                 GetColumn(nameof(PettyCashSheet.BankWithdrawal)),
                 GetColumn(nameof(PettyCashSheet.Expenses)),
                 GetColumn(nameof(PettyCashSheet.BankDeposit)),
                 GetColumn(nameof(PettyCashSheet.NonCashSale)),
                 GetColumn(nameof(PettyCashSheet.Payments)),
                 GetColumn(nameof(PettyCashSheet.CustomerDue)),

            ];
            return GridColumns;
        }
    }
}