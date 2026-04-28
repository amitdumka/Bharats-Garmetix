using Garmetix.Models.Stores;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Stores.PageModels
{
    public class StorePageModel : PageModel<Store>
    {
        public StorePageModel() : base()
        {
            ListView = true;
            try
            {
                DefaultSortedColName = nameof(Store.StartDate);
                DefaultSortedOrder = Ascending;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage("StorePageModel: " + ex.Message);
                SentrySdk.CaptureException(ex);
            }
        }

        /// <summary>
        /// Sets the grid columns
        /// </summary>
        /// <returns></returns>
        protected override ColumnCollection SetGridColumns()
        {
            try
            {
                GridColumns = [
            new DataGridTextColumn() { HeaderText = nameof(Store.Name), MappingName = nameof(Store.Name) },
            new DataGridTextColumn() { HeaderText = nameof(Store.Address), MappingName = nameof(Store.Address) },
            new DataGridTextColumn() { HeaderText = nameof(Store.City), MappingName = nameof(Store.City) },
            new DataGridTextColumn() { HeaderText = nameof(Store.StoreCode), MappingName = nameof(Store.StoreCode) },
            new DataGridTextColumn() { HeaderText = nameof(Store.Active), MappingName = nameof(Store.Active) },
            new DataGridTextColumn() { HeaderText = nameof(Store.StartDate), MappingName = nameof(Store.StartDate), Format = "dd/MMM/yyyy" },
              new DataGridTextColumn() { HeaderText = nameof(Store.Email), MappingName = nameof(Store.Email) },
               this.GetEditDeleteButtons(),//Add the edit and delete buttons
            ];
                return GridColumns;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage("StorePageModel-SetGridColumns: " + ex.Message);
                SentrySdk.CaptureException(ex);
                return
                [
                    new DataGridTextColumn() { HeaderText = "Error", MappingName = "Error" },
                    this.GetEditDeleteButtons()
                ];
            }
            finally
            {
            }
        }
    }
}