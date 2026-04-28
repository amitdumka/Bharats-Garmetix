using Garmetix.Base.PageModels;
using Garmetix.Core.Models.Stores; 
using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Stores.PageModels
{
    public class CompanyPageModel : PageModel<Company>
    {
        public CompanyPageModel() : base()
        {
            ListView = true;

            try
            {
                DefaultSortedColName = nameof(Company.StartDate);
                DefaultSortedOrder = Ascending;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage("CompanyPageModel: " + ex.Message);
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
            new DataGridTextColumn() { HeaderText = nameof(Company.Name), MappingName = nameof(Company.Name) },
            new DataGridTextColumn() { HeaderText = nameof(Company.Address), MappingName = nameof(Company.Address) },
            new DataGridTextColumn() { HeaderText = nameof(Company.City), MappingName = nameof(Company.City) },
            new DataGridTextColumn() { HeaderText = nameof(Company.Code), MappingName = nameof(Company.Code) },
            new DataGridTextColumn() { HeaderText = nameof(Company.Active), MappingName = nameof(Company.Active) },
            new DataGridTextColumn() { HeaderText = nameof(Company.StartDate), MappingName = nameof(Company.StartDate), Format = "dd/MMM/yyyy" },
            new DataGridTextColumn() { HeaderText = nameof(Company.Pan), MappingName = nameof(Company.Pan) },
            new DataGridTextColumn() { HeaderText = nameof(Company.GSTIN), MappingName = nameof(Company.GSTIN) },
            new DataGridTextColumn() { HeaderText = nameof(Company.Email), MappingName = nameof(Company.Email) },
             this.GetEditDeleteButtons(),//Add the edit and delete buttons
            ];
                return GridColumns;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage("CompanyPageModel-SetGridColumns: " + ex.Message);
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

    public class StoreGroupPageModel : PageModel<StoreGroup>
    {
        public StoreGroupPageModel() : base()
        {
            ListView = true;
            DefaultSortedColName = nameof(StoreGroup.Name);
            DefaultSortedOrder = Ascending;
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
            new DataGridTextColumn() { HeaderText = nameof(StoreGroup.Name), MappingName = nameof(StoreGroup.Name) },
            new DataGridTextColumn() { HeaderText = nameof(StoreGroup.GroupCode), MappingName = nameof(StoreGroup.GroupCode) },
            new DataGridTextColumn() { HeaderText = nameof(StoreGroup.StoreCategory), MappingName = nameof(StoreGroup.StoreCategory) },
            new DataGridTextColumn() { HeaderText = nameof(StoreGroup.Active), MappingName = nameof(StoreGroup.Active) },
            this.GetEditDeleteButtons(),//Add the edit and delete buttons
            ];
                return GridColumns;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage("StoreGroupPageModel-SetGridColumns: " + ex.Message);
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