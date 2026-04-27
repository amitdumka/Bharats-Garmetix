using Syncfusion.Maui.DataGrid;
using System.Runtime.CompilerServices;

namespace Garmetix.Core.Views.Customs.Listing
{
    public partial class BaseListingPage : ContentPage
    {
        public int SwipedRowIndex { get; set; } = -1;
        public object SwipedRowData { get; set; } = null;

        public BaseListingPage()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        protected virtual void Datagrid_SwipeEnded(object sender, Syncfusion.Maui.DataGrid.DataGridSwipeEndedEventArgs e)
        {
            if (e.RowData != null)
            {
                SwipedRowIndex = e.RowIndex;
                SwipedRowData = e.RowData;
                OnPropertyChanged(nameof(SwipedRowData));
            }
        }

        protected virtual async void OnSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
        {
#if WINDOWS || MACCATALYST
            if (e.AddedRows.Count > 0)
            {
                //DisplayAlert("Selected", $"Selected: {dataGrid.CurrentRow} ", "OK");
                var selectedItem = e.AddedRows[0]; // Get the selected item

                // Display an action sheet to the user with Edit and Delete options
                string action = await DisplayActionSheet(
                    "Choose Action",
                    "Cancel",
                    null,
                    "Edit", "Delete");

                switch (action)
                {
                    case "Edit":
                        // Execute the Edit command from the ViewModel
                        //if (viewModel.EditCommand.CanExecute(selectedItem))
                        //{
                        //    viewModel.EditCommand.Execute(selectedItem);
                        //}
                        break;

                    case "Delete":
                        // Execute the Delete command from the ViewModel
                        //if (viewModel.DeleteCommand.CanExecute(selectedItem))
                        //{
                        //    viewModel.DeleteCommand.Execute(selectedItem);
                        //}
                        break;
                }

                // De-select the row after the action to allow re-selection of the same row
                dataGrid.SelectedRow = null;

                var x = dataGrid.CurrentRow;
            }
            if (e.AddedRows.Count > 0)
            {
                var selectedRowIndex = dataGrid.SelectedIndex;

                //viewModel.SelectedRowIndex = selectedRowIndex;
            }
#endif
        }

         
    }
}