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

                //new DataGridTextColumn { MappingName = "Id", HeaderText = "ID" },
                //new DataGridTextColumn() { HeaderText = nameof(CustomerDue.InvoiceNumber), MappingName = nameof(CustomerDue.InvoiceNumber) } ,
                //new DataGridTextColumn() { HeaderText = nameof(CustomerDue.Amount), MappingName = nameof(CustomerDue.Amount) } ,
                //new DataGridTextColumn() { HeaderText = nameof(CustomerDue.Paid), MappingName = nameof(CustomerDue.Paid) },
                //new DataGridTextColumn() { HeaderText = nameof(CustomerDue.OnDate), MappingName = nameof(CustomerDue.OnDate), Format = "dd/MMM/yyyy" },
                //new DataGridTextColumn() { HeaderText = nameof(CustomerDue.ClearingDate), MappingName = nameof(CustomerDue.ClearingDate), Format = "dd/MMM/yyyy" },
            ];
            return GridColumns;
        }
    }
}