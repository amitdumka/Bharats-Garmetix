using File = Google.Apis.Drive.v3.Data.File;
namespace Bharat.GoogleDrive.Services
{
    /// <summary>
    /// Defines the contract for a service that interacts with Google Drive.
    /// </summary>
    public interface IGoogleDriveService
    {
        /// <summary>
        /// Uploads a file from a stream to a specified folder path in Google Drive.
        /// Creates the folder path if it doesn't exist.
        /// </summary>
        /// <param name="fileStream">The stream of the file to upload.</param>
        /// <param name="fileName">The desired name of the file in Google Drive.</param>
        /// <param name="folderPath">The path of the folder where the file should be stored (e.g., "Backups/Invoices").</param>
        /// <param name="mimeType">The MIME type of the file (e.g., "application/pdf").</param>
        /// <returns>The ID of the uploaded file, or null if the upload failed.</returns>
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderPath, string mimeType);

        /// <summary>
        /// A specific method to handle uploading a backup of the local SQLite database.
        /// </summary>
        /// <param name="dbPath">The local file path to the SQLite database.</param>
        /// <returns>The ID of the uploaded file, or null if the upload failed.</returns>
        Task<string> UploadDatabaseBackupAsync(string dbPath);

        /// <summary>
        /// A specific method to handle uploading a JSON string as a file.
        /// </summary>
        /// <param name="jsonContent">The JSON content to upload.</param>
        /// <param name="fileName">The name for the JSON file.</param>
        /// <param name="folderPath">The destination folder path in Google Drive.</param>
        /// <returns>The ID of the uploaded file, or null if the upload failed.</returns>
        Task<string> UploadJsonAsync(string jsonContent, string fileName, string folderPath);

        Task<IList<File>> ListFoldersAsync(string folderPath);

        Task<IList<File>> ListFilesAsync(string folderPath);

        Task<IList<File>> ListFilesNewerThanAsync(string folderPath, DateTime lastSyncDate);
        Task<IList<File>> ListFilesNewerThanAsync(string folderId, string folderPath, DateTime lastSyncDate);

        Task<string> DownloadFileAsync(string remoteFilePath, string localFolderPath);
    }
}