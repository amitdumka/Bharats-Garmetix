namespace Garmetix.DataServices.DatabaseBackupServices
{
    public interface IDatabaseBackupService
    {
        /// <summary>
        /// Creates a backup of the current application database.
        /// </summary>
        /// <param name="isAutoBackup">Flag to indicate if the backup is automatic.</param>
        /// <returns>The path to the created backup file.</returns>
        Task<string> BackupDatabaseAsync(bool isAutoBackup = false);

        /// <summary>
        /// Restores the application database from a specified backup file.
        /// </summary>
        Task RestoreDatabaseAsync();

        Task RestoreDatabaseAsync(string filename);

        Task DeleteDatabaseAsync(string fileName);

        /// <summary>
        /// Exports a backup file to a user-selected location.
        /// </summary>
        /// <param name="backupFilePath">The path of the backup file to export.</param>
        Task ExportBackupAsync(string backupFilePath);

        /// <summary>
        /// Starts the long-running task to check for and perform automatic backups.
        /// </summary>
        void StartAutoBackupCheck();

        /// <summary>
        /// Gets a list of available backup files.
        /// </summary>
        /// <returns>A list of backup file paths.</returns>
        List<string> GetBackupFiles();
    }
}