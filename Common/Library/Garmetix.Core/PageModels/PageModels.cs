using Bharat.ToolKits.Extensions;
using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Core.DataModels;
using Garmetix.Core.Sessions;
using Garmetix.Core.Styles;
using Garmetix.DataServices.ImportExports;
using Garmetix.Models.Bases;
using Garmetix.Models.Enums; 
using Syncfusion.Maui.DataGrid;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Garmetix.Core.PageModels
{
    public abstract partial class PageModel<TEntity> : PageModelBase, IPageModel<TEntity> where TEntity : class,  IEntity 
    {
        /// <summary>
        /// Default Sort Order
        /// </summary>
        public const string Ascending = "Ascending";

        /// <summary>
        /// Descending Sort Order
        /// </summary>
        public const string Descending = "Descending";

        #region Properties

        //TODO: reduce no of flag properties, use a single property to track state
        // private bool _isNavigatedTo;
        private bool _dataLoaded;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        protected bool _listView = false;

        // A flag to prevent multiple simultaneous loads when scrolling quickly.
        [ObservableProperty]
        private bool isDataLoading;

        [ObservableProperty]
        protected bool _isPaged = false;

        protected int _currentPage = 1;
#if ANDROID || IOS
        protected const int PageSize = 10; // Number of items to load at a time
#else
        protected const int PageSize = 50; // Number of items to load at a time
#endif

        [ObservableProperty]
        protected int _selectedRowIndex = -1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnableAdd))]
        protected string _addUrl = $"Entry{typeof(TEntity).Name}Page";

        [ObservableProperty]
        protected bool _enableAdd = true;

        [ObservableProperty]
        protected string _defaultSortedColName = "Id";

        [ObservableProperty]
        protected string _defaultSortedOrder = Descending;

        [ObservableProperty]
        protected ObservableCollection<TEntity> _entities;

        [ObservableProperty]
        protected ColumnCollection _gridColumns;

        [ObservableProperty]
        protected int _recordCount = 0;

        [ObservableProperty]
        protected string _title = typeof(TEntity).Name;

        // protected ExcelService ExcelService = ServiceHelper.GetService<ExcelService>(); // Excel Service>
        // protected JsonService JsonService = ServiceHelper.GetService<JsonService>();

        // Data Model
        protected IDataModel<TEntity> DataModel = new DataModel<TEntity>();

        ICommand IPageModel<TEntity>.LoadInitialDataCommand => LoadInitialDataCommand;

        ICommand IPageModel<TEntity>.LoadMoreDataCommand => LoadMoreDataCommand;

        ICommand IPageModel<TEntity>.AddButtonCommand => AddButtonCommand;

        #endregion Properties

        #region Toolbar
        //TODO: Move export and Import Logic to a separate class or single module, Which can be used for Import , Export , Backup and Restore
        //TODO: Just Pass the reference of Enity or Entities to the Export and Import Logic
        // New method for JSON Export
        [RelayCommand]
        protected virtual async Task ExportJson()
        {
            string ReportMessage = "Exporting data to JSON...";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, SessionService.CompanyName(),SessionService.CompanyStoreCode(),Title, $"{Title.Replace(" ", "_")}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_Data.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            ;
            try
            {
                bool success = false;
                if (Entities == null || Entities.Count == 0)
                {
                    var entities = await DataModel.GetAllWithoutLinkAsync();
                    if (entities != null)
                        success=  await entities.ExportToJson(entities, filePath);
                        //success = await JsonService.ExportToJson(entities, filePath);
                }
                else
                {
                    success = await Entities.ExportToJson(Entities.ToList(), filePath);
                   // success = await JsonService.ExportToJson(Entities.ToList(), filePath);
                }
                if (success)
                {
                    ReportMessage = $"Data exported to {filePath}";
                    await Shell.Current.DisplayAlert("Export Complete", ReportMessage, "OK");
                }
                else
                {
                    ReportMessage = "Export Failed: Could not export data to JSON.";
                    await Shell.Current.DisplayAlert("Export Failed", ReportMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ReportMessage = $"An error occurred during JSON export: {ex.Message}";
                await Shell.Current.DisplayAlert("Export Error", ReportMessage, "OK");
            }
        }

        // New method for JSON Import
        [RelayCommand]
        protected virtual async Task ImportJson()
        {
            string ReportMessage = "Selecting JSON file for import...";
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select JSON File",
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                              { DevicePlatform.iOS, new[] { "public.json" } }, // UTType for JSON
                              { DevicePlatform.Android, new[] { "application/json" } },
                              { DevicePlatform.WinUI, new[] { ".json" } },
                              { DevicePlatform.macOS, new[] { "json" } }
                        })
                });

                if (result != null)
                {
                    string filePath = result.FullPath;
                    string report = "";
                    ReportMessage = $"Importing data from {Path.GetFileName(filePath)} (JSON)...";                                       
                    report=await DataImportExport.ImportAndUpdateDatabaseFromJson<TEntity>(filePath,"Sheett1");
                    ReportMessage = report;
                    await Shell.Current.DisplayAlert("Import Report (JSON)", report, "OK");
                }
                else
                {
                    ReportMessage = "JSON Import Cancelled: No file was selected.";
                    await Shell.Current.DisplayAlert("Import Cancelled", ReportMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ReportMessage = $"An error occurred during JSON file picking or import: {ex.Message}";
                await Shell.Current.DisplayAlert("Import Error (JSON)", ReportMessage, "OK");
            }
        }

        [RelayCommand]
        protected virtual async Task ExportToExcelAsync()
        {
            string ReportMessage = "Exporting data...";
            // // In a real MAUI app, you'd use FilePicker or similar to get a save path
            // // For simplicity, let's use a temporary path for demonstration.
            // // You might want to use CommunityToolkit.Maui.Storage.FileSaver for a better UX.
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"{Title.Replace(" ", "_")}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_Data.xlsx");

            try
            {
                //     // Fetch data from the database
                //     var products = await _dbContext.Products.ToListAsync();
                bool success = false;
                if (Entities == null || Entities.Count == 0)
                {
                    var entities = await DataModel.GetAllWithoutLinkAsync();
                    success = entities.ExportToExcel(entities, filePath);
                }
                else
                {
                    success = Entities.ExportToExcel(Entities.ToList(), filePath);
                }

                if (success)
                {
                    ReportMessage = $"Data exported to {filePath}";
                    await Shell.Current.DisplayAlert("Export Complete", ReportMessage, "OK");
                }
                else
                {
                    ReportMessage = "Export Failed: Could not export data to Excel.";
                    await Shell.Current.DisplayAlert("Export Failed", ReportMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ReportMessage = $"An error occurred during export: {ex.Message}";
                await Shell.Current.DisplayAlert("Export Error", ReportMessage, "OK");
            }
        }

        [RelayCommand]
        public virtual async Task ImportFromExcelAsync()
        {
            string ReportMessage = "Selecting file for import...";
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Excel File",
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.iOS, new[] { "com.microsoft.excel.xlsx" } }, // UTType for XLSX
                            { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                            { DevicePlatform.WinUI, new[] { ".xlsx" } },
                            { DevicePlatform.macOS, new[] { "xlsx" } }
                        })
                });

                if (result != null)
                {
                    string filePath = result.FullPath;
                    string report = "";
                    ReportMessage = $"Importing data from {Path.GetFileName(filePath)}...";
                    TEntity? entity = null;
                    report = await DataImportExport.ImportAndUpdateDatabaseFromExcel<TEntity>(filePath,"Sheett1");
                    ReportMessage = report; // Display the detailed report
                    await Shell.Current.DisplayAlert("Import Report", report, "OK");
                }
                else
                {
                    ReportMessage = "Import Cancelled: No file was selected.";
                    await Shell.Current.DisplayAlert("Import Cancelled", ReportMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ReportMessage = $"An error occurred during file picking or import: {ex.Message}";
                await Shell.Current.DisplayAlert("Import Error", ReportMessage, "OK");
            }
        }

        #endregion Toolbar

        public PageModel()
        {
            try
            {
                _role = StorageOps.GetPref("UserType", UserType.Admin); //TODO: Use this method to set user role and other
                _entities = [];
                _addUrl = $"Entry{typeof(TEntity).Name}Page";
                // Setting Icon For page
                Icon = IconFont.UserCheck;
                //Setting Title from Entity Name
                Title = typeof(TEntity).Name.SplitPascalCase_Simple();
                //Setting Default Sorted Column
                DefaultSortedOrder = Descending;
                if (!ListView)
                {
                    //Setting Grid View Columns for Dynamic Fields
                    _gridColumns = [];
                    SetGridColumns();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage("PageModel-Constructor" + ex.Message);
                SentrySdk.CaptureException(ex);
                Notify.ShowError($"{typeof(TEntity).Name} PM", ex, snabackbar: true);
            }
        }

        #region EditDelete

        public override void Edit(Object itemToEdit)
        {
            TEntity entity = (TEntity)itemToEdit;
            if (entity != null)
            {
                _ = Shell.Current.GoToAsync($"{AddUrl}?Id={entity.Id}");
            }
            else
            {
                _ = Notify.ShowError($"{typeof(TEntity).Name} PM", ex: new ArgumentNullException("itemToEdit"), snabackbar: true);
            }
        }

        public override void Delete(Object itemToDelete)
        {
            TEntity entity = (TEntity)itemToDelete;
            if (entity != null)
            {
                Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this record?", "Yes", "No").ContinueWith((result) =>
                {
                    if (result.Result == true)
                    {
                        var deleteResult = DataModel.DeleteAsync(entity.Id).Result;
                        if (deleteResult)
                        {
                            Entities.Remove(entity);
                            Notify.ShowSuccess($"{typeof(TEntity).Name} PM", "Record Deleted Successfully", snabackbar: true, speak: true, isLong: true);
                        }
                    }
                });
            }
        }

        #endregion EditDelete

        #region DataLoding

        [RelayCommand]
        private async Task LoadMoreDataAsync()
        {
            if (IsBusy || IsDataLoading)
            {
                return;
            }

            try
            {
                IsDataLoading = true;
                _currentPage++; // Go to the next page
                if (ListView || IsPaged)
                {
                    var items = await DataModel.GetAllAsync(_currentPage, PageSize);

                    if (items != null && items.Count != 0)
                    {
                        foreach (var item in items)
                        {
                            Entities.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Notify.ShowError($"{typeof(TEntity).Name} PM", ex, snabackbar: false);
                SentrySdk.CaptureMessage("PageModel-LoadMoreDataAsync" + ex.Message);
                SentrySdk.CaptureException(ex);
            }
            finally
            {
                RecordCount = Entities.Count;
                IsDataLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadInitialDataAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                _currentPage = 1;
                Entities.Clear();
                if (ListView || IsPaged)
                {
                    var items = await DataModel.GetAllAsync(_currentPage, PageSize);
                    if (items == null
                        && items.Count == 0)
                    {
                        return;
                    }
                    foreach (var item in items)
                    {
                        Entities.Add(item);
                    }
                }
                else
                {
                    var items = await DataModel.GetAllAsync();
                    if (items == null
                        && items.Count == 0)
                    {
                        Debug.WriteLine($"items is empty or null");
                        _=Notify.ShowError($"{typeof(TEntity).Name} PM", "items is empty or null", snabackbar: false);
                    }
                    else
                    {
                        foreach (var item in items)
                        {
                            Entities.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Debug.WriteLine($"Error loading initial data: {ex.Message}");

                // Here you would show an error message to the user
                SentrySdk.CaptureMessage("PageModel-LoadInitialDataAsync" + ex.Message);
                _ = Notify.ShowError($"{typeof(TEntity).Name} PM", ex, snabackbar: false);
                SentrySdk.CaptureException(ex);
                IsBusy = false;
            }
            finally
            {
                RecordCount = Entities.Count;
                IsBusy = false;
            }
        }

        #endregion DataLoding

        #region Abstract Methods

        /// <summary>
        /// This method is used to set the grid columns, it is an abstract method
        /// </summary>
        /// <returns></returns>
        protected abstract ColumnCollection SetGridColumns();

        #endregion Abstract Methods

        #region Methods

        //[RelayCommand]
        private void NavigatedTo() =>
            _isNavigatedTo = true;

        //[RelayCommand]
        private void NavigatedFrom() =>
            _isNavigatedTo = false;

        //[RelayCommand]
        public override async Task Appearing()
        {
            if (!_dataLoaded)
            {
                await HandleOnOnAppearing();
                _dataLoaded = true;
            }
            // This means we are being navigated to
            else if (!_isNavigatedTo)
            {
                await Refresh();
            }
        }

        public async Task HandleOnOnAppearing()
        {
            try
            {
                if (Entities.Count == 0)
                {
                    await LoadInitialDataAsync();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.ConfigureScope(scope => scope.SetTag("Page", GetType().Name));
                SentrySdk.CaptureException(ex);
                SentrySdk.CaptureMessage($"{GetType().Name} OnApperaing Exp" + ex.Message);
                await Notify.ShowError($"{typeof(TEntity).Name} PM", ex, snabackbar: true);
                // await Notify.DisplayNotificationAsync("Failed to load Store data. Please try again later.");
            }
        }

        protected DataGridTextColumn AddGridColumns(string name)
        {
            return new DataGridTextColumn() { HeaderText = name.SplitPascalCase_Simple(), MappingName = name };
        }

        protected DataGridTextColumn AddDateGridColumns(string name)
        {
            return new DataGridTextColumn() { HeaderText = name.SplitPascalCase_Simple(), MappingName = name, Format = "dd/MMM/yyyy" };
        }

        public static DataGridColumn GetColumn(string name, bool isDate = false)
        {
            return new DataGridTextColumn
            {
                HeaderText = name,
                MappingName = name,
                Format = isDate ? "dd/MMM/yyyy" : "{0}",
            };
        }

        /// <summary>
        /// Generating Edit and Delete Buttons for DataGrid
        /// </summary>
        /// <returns></returns>
        protected DataGridTemplateColumn GetEditDeleteButtons()
        {
            return new DataGridTemplateColumn
            {
                MappingName = "Actions",
                HeaderText = "Actions",
                CellPadding = 10,
                CellTextAlignment = TextAlignment.Center,
                HeaderPadding = 10,
                MinimumWidth = 200,
                Format = "Actions",
                ColumnWidthMode = ColumnWidthMode.FitByCell,

                CellTemplate = new DataTemplate(() =>
                {
                    var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = 1, Padding = 1 };
                    var editButton = new Button { Text = "Edit", Margin = 5, Scale = 0.7 };
                    editButton.SetBinding(Button.CommandParameterProperty, ".");
                    editButton.Clicked += (s, e) => { /* Handle edit */ };
                    Button deleteButton = new() { Text = "Delete", Margin = 5, Scale = 0.7 };
                    deleteButton.SetBinding(Button.CommandParameterProperty, ".");
                    deleteButton.Clicked += (s, e) => { /* Handle delete */ };
                    stack.Children.Add(editButton);
                    stack.Children.Add(deleteButton);
                    return stack;
                })
            };
        }

        /// <summary>
        /// This method is used to refresh the datagrid it can be used to reload the data
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        protected async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                await LoadInitialDataAsync();
            }
            catch (Exception e)
            {
                _ = Notify.ShowError($"{typeof(TEntity).Name} PM", e, snabackbar: true);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// Add Button Command, It handle to navigate or show Add/Edit Form View
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        protected async Task AddButton()
        {
            try
            {
                if (EnableAdd)
                {
                    await Shell.Current.GoToAsync($"{AddUrl}");
                }
                else
                {
                    _ = Notify.DisplayNotificationAsync("You are not allowed to add new record");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                //_ = Notify.DisplayNotificationAsync($"Error: {ex.Message}");
                _=Notify.ShowError($"{typeof(TEntity).Name} Add Button", ex, snabackbar: false);
                await Shell.Current.GoToAsync("///main");
            }
        }

        #endregion Methods


        
    }
}