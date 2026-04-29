using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Models.Reports;
using System.Collections.ObjectModel;

namespace Garmetix.Reports.PageModels
{
    public partial class ReprintPageModel : BasePageModel
    {
        [ObservableProperty]
        private ObservableCollection<string> _reportOptions =
            [
                "Voucher",
                "SalaryPayment",
                "Ledgers"
            ];

        [ObservableProperty]
        private string _selectedFile = string.Empty;
        [ObservableProperty]
        private BackupFile _selectedItem = null;

        [ObservableProperty]
        private string _selectedOption = "Voucher";

        [ObservableProperty]
        private ObservableCollection<BackupFile> _reportPdfFiles = [];

        public ReprintPageModel()
        {
            ReportOptions =
            [
                "Voucher",
                "SalaryPayment",
                "Ledgers"
            ];
        }

        public List<string> GetPdfFiles(string options)
        {
            try
            {
                return Directory.GetFiles(Path.Combine(FileSystem.CacheDirectory, "BharatGarmetix", options), "*.pdf").ToList();
            }
            catch (Exception ex)
            {
                _ = Notify.DisplayNotificationAsync(ex.Message );
                return new List<string>();
            }
        }

        public async Task LoadPdfFilesAsync(string options = "Vouchers")
        {
            IsBusy = true; // Set busy state to true while loading
            ReportPdfFiles.Clear(); // Clear existing items before loading new ones

            try
            {
                // Simulate a network or file system delay
                await Task.Delay(10);

                var files = GetPdfFiles(options);
                foreach (var file in files)
                {
                    ReportPdfFiles.Add(new BackupFile { FileName = Path.GetFileName(file), FilePath = Path.GetFullPath(file) });
                }
                if (files.Count > 0)
                {
                    SelectedItem=ReportPdfFiles[0];
                    SelectedFile = ReportPdfFiles[0].FilePath;
                    _ = Notify.DisplayNotificationAsync($"Loaded {files.Count} files", isLong: true);
                }
                else
                {
                    _ = Notify.DisplayNotificationAsync($"No files found", isLong: true);
                }
            }
            finally
            {
                IsBusy = false; // Always set busy state to false when loading is complete (or fails)
            }
        }

        private bool CanPrintPdf()
        {
            return SelectedFile != null;
        }

        private bool CanGetFiles()
        {
            return SelectedOption != null;
        }

        [RelayCommand]
        private async Task LoadPdfFiles()
        {
            await LoadPdfFilesAsync(SelectedOption);
        }

        [RelayCommand]
        private async Task PrintFileAsync()
        {
            if (string.IsNullOrEmpty(SelectedItem.FilePath))
            {
                return;
            }

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(SelectedItem.FilePath)
            });
        }
    }
}