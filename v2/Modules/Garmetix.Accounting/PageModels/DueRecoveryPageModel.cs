using Garmetix.Base.PageModels;
using Garmetix.Core.Models.Accounting;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Accounting.PageModels
{
    public class DueRecoveryPageModel : PageModel<DueRecovery>
    {
        public DueRecoveryPageModel() : base()
        {
            DefaultSortedColName = nameof(DueRecovery.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns = [
                AddGridColumns(nameof(DueRecovery.InvoiceNumber)),
                AddGridColumns(nameof(DueRecovery.Amount)),
                AddGridColumns(nameof(DueRecovery.Paid)),
                AddDateGridColumns(nameof(DueRecovery.OnDate)),
                AddDateGridColumns(nameof(DueRecovery.ClearingDate)),

                //new DataGridTextColumn { MappingName = "Id", HeaderText = "ID" },
                //new DataGridTextColumn() { HeaderText = nameof(DueRecovery.InvoiceNumber), MappingName = nameof(DueRecovery.InvoiceNumber) } ,
                //new DataGridTextColumn() { HeaderText = nameof(DueRecovery.Amount), MappingName = nameof(DueRecovery.Amount) } ,
                //new DataGridTextColumn() { HeaderText = nameof(DueRecovery.Paid), MappingName = nameof(DueRecovery.Paid) },
                //new DataGridTextColumn() { HeaderText = nameof(DueRecovery.OnDate), MappingName = nameof(DueRecovery.OnDate), Format = "dd/MMM/yyyy" },
                //new DataGridTextColumn() { HeaderText = nameof(DueRecovery.ClearingDate), MappingName = nameof(DueRecovery.ClearingDate), Format = "dd/MMM/yyyy" },
            ];
            return GridColumns;
        }
    }
}