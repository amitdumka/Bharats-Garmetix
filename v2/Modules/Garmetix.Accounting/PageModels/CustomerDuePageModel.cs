using Garmetix.Base.PageModels;
using Garmetix.Core.Models.Accounting;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public class CustomerDuePageModel : PageModel<CustomerDue>
    {
        public CustomerDuePageModel() : base()
        {
            DefaultSortedColName = nameof(CustomerDue.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns = [
                AddGridColumns(nameof(CustomerDue.InvoiceNumber)),
                AddGridColumns(nameof(CustomerDue.Amount)),
                AddGridColumns(nameof(CustomerDue.Paid)),
                AddDateGridColumns(nameof(CustomerDue.OnDate)),
                AddDateGridColumns(nameof(CustomerDue.ClearingDate)),

               ];
            return GridColumns;
        }
    }
}