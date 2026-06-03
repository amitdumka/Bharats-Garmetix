using Android.Telephony.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.ImportExports.Services;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Garmetix.POS.Desktop.ViewModels
{
    public partial class PurchaseImportViewModel : ObservableObject
    {
        private readonly PurchaseImportService _importService;

        [ObservableProperty] private string _selectedExcelFilePath;
        [ObservableProperty] private string _outputDirectoryPath;
        [ObservableProperty] private string _statusMessage = "Ready to import.";
        [ObservableProperty] private bool _isProcessing;

        // Granular Database Import Options
        [ObservableProperty] private bool _importProducts = true;
        [ObservableProperty] private bool _importStock = true;
        [ObservableProperty] private bool _importPurchaseInvoices = true;

        public PurchaseImportViewModel()
        {
            _importService = new PurchaseImportService();
            OutputDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        [RelayCommand]
        public async Task SelectExcelFileAsync()
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xlsx", ".xls" } },
                    { DevicePlatform.MacCatalyst, new[] { "xlsx", "xls" } }
                });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Purchase/Inward Excel Export",
                    FileTypes = customFileType
                });

                if (result != null)
                {
                    SelectedExcelFilePath = result.FullPath;
                    StatusMessage = $"Selected: {result.FileName}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"File selection failed: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task SelectOutputFolderAsync()
        {
            try
            {
                var result = await FolderPicker.Default.PickAsync();
                if (result != null && result.IsSuccessful)
                {
                    OutputDirectoryPath = result.Folder.Path;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Folder selection failed: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task StartImportAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedExcelFilePath) || !File.Exists(SelectedExcelFilePath))
            {
                StatusMessage = "Error: Please select a valid Excel file first.";
                return;
            }

            IsProcessing = true;
            StatusMessage = "Reading Excel & Generating JSON Backup...";

            try
            {
                await Task.Run(async () =>
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string outputFilePath = Path.Combine(OutputDirectoryPath, $"PurchaseInward_{timestamp}.json");

                    // 1. Generate JSON
                    _importService.GeneratePurchaseImportJson(SelectedExcelFilePath, outputFilePath);

                    // 2. Process DB Inserts if requested
                    if (ImportProducts || ImportStock || ImportPurchaseInvoices)
                    {
                        MainThread.BeginInvokeOnMainThread(() => StatusMessage = "Pushing selected records to ERP Database...");

                        // TODO: Call your backend API or DbContext service here, passing the outputFilePath
                        // Example: await _databaseService.ProcessPurchaseImportAsync(outputFilePath, ImportProducts, ImportStock, ImportPurchaseInvoices);

                        if (ImportProducts || ImportStock || ImportPurchaseInvoices)
                        {
                            MainThread.BeginInvokeOnMainThread(() => StatusMessage = "Pushing selected records to ERP Database...");

                            // In a real MAUI app, inject your Database Sync Service via dependency injection
                            // For this example, assuming you have access to the DB Context or an API client:

                            var syncService = new PurchaseDatabaseSyncService(_dbContext);

                            // Pass the StoreId for the current logged-in session
                            Guid currentStoreId = AppSettings.CurrentStoreId;

                            await syncService.ProcessImportAsync(
                                outputFilePath,
                                ImportProducts,
                                ImportStock,
                                ImportPurchaseInvoices,
                                currentStoreId);
                        }

                    }
                });

                StatusMessage = $"✅ Success! JSON saved to {OutputDirectoryPath}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Import Failed: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}