using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Garmetix.Base.Views.Customs.Listing
{
    public partial class BaseListViewPage : ContentPage
    {
        public static readonly BindableProperty ListItemTemplateProperty =
            BindableProperty.Create(nameof(ListItemTemplate), typeof(DataTemplate), typeof(BaseListViewPage), default(DataTemplate));

        public DataTemplate ListItemTemplate
        {
            get => (DataTemplate)GetValue(ListItemTemplateProperty);
            set => SetValue(ListItemTemplateProperty, value);
        }
        public int SwipedRowIndex { get; set; } = -1;
        public object SwipedRowData { get; set; } = null;

        public BaseListViewPage()
        {
            InitializeComponent();
            if (ListItemTemplate != null)
            {
                ColView.ItemTemplate = ListItemTemplate;
                DisplayAlertAsync("Info Constructor", "ListItemTemplate is SET", "OK");
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Debug check: log the current value of ListItemTemplate.
            Debug.WriteLine($"OnAppearing: ListItemTemplate is {(ListItemTemplate == null ? "NULL" : "SET")}.");
          // DisplayAlert("Info Debug", $"OnAppearing: ListItemTemplate is {(ListItemTemplate == null ? "NULL" : "SET")}.", "OK");
            if (ListItemTemplate != null)
            {
                ColView.ItemTemplate = ListItemTemplate;
            }

            if(ListItemTemplate==null)
                DisplayAlertAsync("Error", "ListItemTemplate is NULL", "OK");
            // Optionally, use a breakpoint here to inspect ListItemTemplate in the debugger.
            // If you see "NULL", then the DataTemplate hasn't been applied yet.
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

        protected virtual void OnSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
        {
             
        }

        protected virtual async void OnAddClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Add", "Add button clicked!", "OK");
        }

        protected virtual async void OnDeleteClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Delete", "Delete button clicked!", "OK");
        }

        protected virtual async void OnExportClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Export", "Export button clicked!", "OK");
        }
    }
}