using Bharat.GoogleDrive.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; 
using System.Text.Json;

namespace Bharat.GoogleDrive.ViewModels
{
    public partial class GoogleDrivePageModel : ObservableObject
    {
        private readonly IGoogleDriveService _googleDriveService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private string _statusMessage = "Ready";

        [ObservableProperty]
        private Color _statusColor = Colors.Gray;

        public GoogleDrivePageModel(IGoogleDriveService googleDriveService)
        {
            _googleDriveService = googleDriveService;
        }

        [RelayCommand]
        private async Task UploadDbBackupAsync()
        {
            await PerformUpload(async () =>
            {
                // In a real app, get this path from your database configuration
                string dbFileName = "myapp.db3";
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, dbFileName);

                // For demonstration, create a dummy file if it doesn't exist
                if (!File.Exists(dbPath))
                {
                    await File.WriteAllTextAsync(dbPath, "This is a dummy SQLite database file.");
                }

                return await _googleDriveService.UploadDatabaseBackupAsync(dbPath);
            }, "Database Backup");
        }

        [RelayCommand]
        private async Task UploadInvoiceAsync()
        {
            await PerformUpload(async () =>
            {
                // For demonstration, we create a dummy PDF file stream in memory.
                // In a real app, you would generate or read a real PDF file stream.
                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream);
                writer.Write("%PDF-1.4\n1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n2 0 obj<</Type/Pages/Count 1/Kids[3 0 R]>>endobj\n3 0 obj<</Type/Page/MediaBox[0 0 612 792]>>endobj\nxref\n0 4\n0000000000 65535 f\n0000000009 00000 n\n0000000052 00000 n\n0000000101 00000 n\ntrailer<</Size 4/Root 1 0 R>>\nstartxref\n155\n%%EOF");
                writer.Flush();
                stream.Position = 0;

                string fileName = $"invoice_{DateTime.Now:yyyyMMdd}_{new Random().Next(100, 999)}.pdf";
                return await _googleDriveService.UploadFileAsync(stream, fileName, "AppBackups/Invoices", "application/pdf");
            }, "Invoice PDF");
        }

        [RelayCommand]
        private async Task UploadJsonAsync()
        {
            await PerformUpload(async () =>
            {
                // Create some sample data
                var data = new { VoucherId = 12345, Amount = 99.99, Date = DateTime.UtcNow, Items = new[] { "Item A", "Item B" } };
                string jsonContent = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                string fileName = $"voucher_{data.VoucherId}.json";

                return await _googleDriveService.UploadJsonAsync(jsonContent, fileName, "AppBackups/Vouchers");
            }, "Voucher JSON");
        }

        private async Task PerformUpload(Func<Task<string>> uploadAction, string uploadType)
        {
            if (IsBusy) return;

            IsBusy = true;
            StatusMessage = $"Uploading {uploadType}...";
            StatusColor = Colors.Orange;

            string fileId = await uploadAction.Invoke();

            if (!string.IsNullOrEmpty(fileId))
            {
                StatusMessage = $"{uploadType} uploaded successfully!";
                StatusColor = Colors.Green;
            }
            else
            {
                StatusMessage = $"Failed to upload {uploadType}. Check logs for details.";
                StatusColor = Colors.Red;
            }

            IsBusy = false;
        }
    }
}
