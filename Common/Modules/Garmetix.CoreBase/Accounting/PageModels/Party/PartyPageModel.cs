using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Accounting.PageModels
{
    public class PartyPageModel : PageModel<Party>
    {
        public PartyPageModel() : base()
        {
            DefaultSortedColName = nameof(Party.Name);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                AddGridColumns(nameof(Party.Name)),
                AddGridColumns(nameof(Party.LedgerId)),
                AddGridColumns(nameof(Party.PAN)),
                AddGridColumns(nameof(Party.GSTIN)),
                AddGridColumns(nameof(Party.Address)),
                AddGridColumns(nameof(Party.Phone)),
                AddGridColumns(nameof(Party.EmailId)),

                //new DataGridTextColumn() { HeaderText = nameof(Party.Name), MappingName = nameof(Party.Name) },
                //new DataGridTextColumn() { HeaderText = nameof(Party.LedgerId), MappingName = nameof(Party.LedgerId) },
                //new DataGridTextColumn() { HeaderText = nameof(Party.PAN), MappingName = nameof(Party.PAN) },
                //new DataGridTextColumn() { HeaderText = nameof(Party.GSTIN), MappingName = nameof(Party.GSTIN) },
                //new DataGridTextColumn() { HeaderText = nameof(Party.Address), MappingName = nameof(Party.Address) },
                //new DataGridTextColumn() { HeaderText = nameof(Party.Phone), MappingName = nameof(Party.Phone) },
                //new DataGridTextColumn() { HeaderText = nameof(Party.EmailId), MappingName = nameof(Party.EmailId) },
            ];

            return GridColumns;
        }
    }
}