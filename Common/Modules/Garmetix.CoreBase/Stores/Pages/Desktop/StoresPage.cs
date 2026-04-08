using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.Core.Views.Customs.Listing;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.Stores.Pages.Desktop
{
    public class StoresPage : BaseListingPage
    {
        private readonly StorePageModel _viewModel;
        public StoresPage(StorePageModel vm)
        {
            try
            {
                Title = "Stores";
                //vm.AddUrl = $"Entry{nameof(Store)}Page";
                BindingContext = _viewModel = vm;
            }
            catch (Exception ex)
            {

                _ = SentrySdk.CaptureException(ex);
                _ = SentrySdk.CaptureMessage("StorePPage Exp" + ex.Message);
            }
            finally
            {
            }
        }

         

        protected override void Datagrid_SwipeEnded(object sender, DataGridSwipeEndedEventArgs e)
        {
            // base.Datagrid_SwipeEnded(sender, e);
            if (e.RowData != null)
            {
                SwipedRowIndex = e.RowIndex;
                SwipedRowData = e.RowData;
                OnPropertyChanged(nameof(SwipedRowData));
            }
        }

        protected override void OnSelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            SfDataGrid dataGrid = (SfDataGrid)sender;
            if (e.AddedRows.Count > 0)
            {

                DisplayAlert("Stores", $"Store Selected: {dataGrid.CurrentRow} ", "OK");
                var x = dataGrid.CurrentRow;
            }
            if (e.AddedRows.Count > 0)
            {
                var selectedRowIndex = dataGrid.SelectedIndex;

                _viewModel.SelectedRowIndex = selectedRowIndex;

            }
        }
    }
}