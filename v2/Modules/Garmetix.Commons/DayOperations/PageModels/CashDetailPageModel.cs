using Garmetix.Base.PageModels;
using Garmetix.Models.DayOperations;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Commons.DayOperations.PageModels
{
    public class CashDetailPageModel : PageModel<CashDetail>
    {
        public CashDetailPageModel()
        {
            DefaultSortedColName = nameof(CashDetail.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
          [
                GetColumn(nameof(CashDetail.OnDate)),
                 GetColumn(nameof(CashDetail.Amount)),
                 GetColumn(nameof(CashDetail.N2000)),
                 GetColumn(nameof(CashDetail.N500)),
                 GetColumn(nameof(CashDetail.N200)),
                 GetColumn(nameof(CashDetail.N100)),
                 GetColumn(nameof(CashDetail.N50)),
                 GetColumn(nameof(CashDetail.NC20)),
                 GetColumn(nameof(CashDetail.NC10)),
                 GetColumn(nameof(CashDetail.NC5)),
                 GetColumn(nameof(CashDetail.NC2)),
                 GetColumn(nameof(CashDetail.NC1)),

            ];
            return GridColumns;
        }
    }
}