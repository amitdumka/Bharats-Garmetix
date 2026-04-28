using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Accounting.PageModels
{
    public class BankPageModel : PageModel<Bank>
    {
        public BankPageModel() : base()
        {
            DefaultSortedColName = nameof(Bank.Name);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                //GridColumns.Add(new DataGridTextColumn() { HeaderText = nameof(Bank.Id), MappingName = nameof(Bank.Id) });
                new DataGridTextColumn() { HeaderText = nameof(Bank.Name), MappingName = nameof(Bank.Name) },
            ];

            return GridColumns;
        }
    }
}