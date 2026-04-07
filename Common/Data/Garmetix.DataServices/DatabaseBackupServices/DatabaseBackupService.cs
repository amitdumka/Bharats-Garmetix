using Garmetix.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Garmetix.DataServices.DatabaseBackupServices
{
    // Note: To start the auto-backup service, resolve IDatabaseBackupService
    // from the service container after building the app in MauiProgram.cs and call
    // StartAutoBackupCheck().
    // Example in MauiProgram.cs:
    // var app = builder.Build();
    // var backupService = app.Services.GetRequiredService<IDatabaseBackupService>();
    // backupService.StartAutoBackupCheck();
    // return app;
    public class DatabaseBackupService : IDatabaseBackupService, IDisposable
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<DatabaseBackupService> _logger;
        private Timer _autoBackupTimer;
        private const string LastBackupDateKey = "LastAutoBackupDate";
        private readonly string _backupDir;

        public DatabaseBackupService(DatabaseContext dbContext, ILogger<DatabaseBackupService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _backupDir = Path.Combine(FileSystem.AppDataDirectory, "Backups");
            if (!Directory.Exists(_backupDir))
            {
                Directory.CreateDirectory(_backupDir);
            }
        }

        public void StartAutoBackupCheck()
        {
            _logger.LogInformation("Auto-backup service starting.");
            _autoBackupTimer = new Timer(
                callback: async _ => await CheckAndPerformAutoBackupAsync(),
                state: null,
                dueTime: TimeSpan.FromSeconds(10),
                period: TimeSpan.FromHours(6)
            );
        }

        private async Task CheckAndPerformAutoBackupAsync()
        {
            try
            {
                _logger.LogInformation("Performing automatic backup check.");
                var lastBackupString = Preferences.Get(LastBackupDateKey, string.Empty);

                bool shouldBackup = string.IsNullOrEmpty(lastBackupString) ||
                                    (DateTime.UtcNow - DateTime.Parse(lastBackupString, null, System.Globalization.DateTimeStyles.RoundtripKind)).TotalDays >= 7;

                if (shouldBackup)
                {
                    _logger.LogInformation("Initiating new auto-backup.");
                    await BackupDatabaseAsync(isAutoBackup: true);
                }
                else
                {
                    _logger.LogInformation("No auto-backup needed at this time.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the automatic backup check.");
            }
        }

        public async Task<string> BackupDatabaseAsync(bool isAutoBackup = false)
        {
            var backupFileName = $"backup_{(isAutoBackup ? "AUTO_" : "")}_{Path.GetFileName(Constants.CompanyDatabaseFileName)}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.db";
            var backupFilePath = Path.Combine(_backupDir, backupFileName);
            var currentDbPath = _dbContext.DatabasePath;

            _logger.LogInformation("Starting database backup from {Source} to {Destination}", currentDbPath, backupFilePath);

            try
            {
                await _dbContext.Database.CloseConnectionAsync();
                File.Copy(currentDbPath, backupFilePath, true);
                _logger.LogInformation("Database backup completed successfully.");

                if (isAutoBackup)
                {
                    Preferences.Set(LastBackupDateKey, DateTime.UtcNow.ToString("o"));
                    _logger.LogInformation("Updated last auto-backup date to {Date}", DateTime.UtcNow);
                }
                _dbContext.Database.OpenConnection();
                return backupFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database backup.");
                throw;
            }
        }

        public async Task ExportBackupAsync(string backupFilePath)
        {
            if (!File.Exists(backupFilePath))
            {
                throw new FileNotFoundException("Backup file not found.", backupFilePath);
            }

            // Using FileSaver from CommunityToolkit to save the file to a public location.
            // This handles the platform-specific complexities of file system access.
            using var stream = File.OpenRead(backupFilePath);
            var fileSaverResult = await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync(Path.GetFileName(backupFilePath), stream);
            if (!fileSaverResult.IsSuccessful)
            {
                throw new Exception($"Failed to save file: {fileSaverResult.Exception?.Message}");
            }
        }

        public async Task RestoreDatabaseAsync()
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a backup file (.db)",
                // Optionally, define file types to make selection easier
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                 {
                     { DevicePlatform.iOS, new[] { "public.database" } },
                     { DevicePlatform.Android, new[] { "application/vnd.sqlite3", "application/x-sqlite3" } },
                     { DevicePlatform.WinUI, new[] { ".db" } },
                     { DevicePlatform.macOS, new[] { "db" } },
                 })
            });

            if (fileResult == null)
            {
                return; // User cancelled the picker
            }

            var currentDbPath = _dbContext.DatabasePath;

            _logger.LogInformation("Starting database restore from {Source} to {Destination}", fileResult.FullPath, currentDbPath);

            try
            {
                await _dbContext.Database.CloseConnectionAsync();

                Microsoft.Data.Sqlite.SqliteConnection.ClearPool((Microsoft.Data.Sqlite.SqliteConnection)_dbContext.Database.GetDbConnection());
                GC.Collect();
                GC.WaitForPendingFinalizers();

                File.Copy(fileResult.FullPath, currentDbPath, true);
                _logger.LogInformation("Database restore completed successfully. App restart is recommended.");
                _dbContext.Database.OpenConnection();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database restore.");
                throw;
            }
        }

        public Task DeleteDatabaseAsync(string fileName)
        {
            if (fileName == null)
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation("Deleting database restore from {Source}  ", fileName);

            try
            {
                File.Delete(fileName);
                _logger.LogInformation("Database restore completed successfully. App restart is recommended.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database restore.");
                throw;
            }

            return Task.CompletedTask;
        }

        public async Task RestoreDatabaseAsync(string fileName)
        {
            if (fileName == null)
            {
                return; // User cancelled the picker
            }

            var currentDbPath = _dbContext.DatabasePath;

            _logger.LogInformation("Starting database restore from {Source} to {Destination}", fileName, currentDbPath);

            try
            {
                await _dbContext.Database.CloseConnectionAsync();

                Microsoft.Data.Sqlite.SqliteConnection.ClearPool((Microsoft.Data.Sqlite.SqliteConnection)_dbContext.Database.GetDbConnection());
                GC.Collect();
                GC.WaitForPendingFinalizers();

                File.Copy(fileName, currentDbPath, true);
                _logger.LogInformation("Database restore completed successfully. App restart is recommended.");
                _dbContext.Database.OpenConnection();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database restore.");
                throw;
            }
        }

        public List<string> GetBackupFiles()
        {
            return Directory.GetFiles(_backupDir).ToList();
        }

        public void Dispose()
        {
            _autoBackupTimer?.Dispose();
        }
    }
}