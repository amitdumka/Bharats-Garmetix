using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Reflection;
using System.Text;
using File = Google.Apis.Drive.v3.Data.File; // Alias to avoid conflict with System.IO.File

namespace Bharat.GoogleDrive.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private const string AppName = "Garmetix"; // Replace with your app name        
        private readonly IDataStore _dataStore = new MauiSecureDataStore();
        // Scope changed to Drive to allow listing and reading all files, not just app-created ones.
        private readonly string[] Scopes = { DriveService.Scope.Drive };
        //private readonly string[] Scopes = { DriveService.Scope.DriveFile };
       
         
        private async Task<DriveService> GetDriveServiceAsync()
        {
            UserCredential credential;

            // Load client secrets from embedded resource
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith("client_secrets.json"));
            
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new FileNotFoundException("client_secrets.json not found as an embedded resource.");
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user", // A unique identifier for the user.
                    CancellationToken.None,
                    _dataStore
                );
            }

            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });
        }

        private async Task<string> GetOrCreateFolderIdAsync(DriveService service, string folderPath)
        {
            string parentId = "root";
            var folderNames = folderPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var name in folderNames)
            {
                var listRequest = service.Files.List();
                listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and trashed=false and name='{name}' and '{parentId}' in parents";
                listRequest.Fields = "files(id)";
                var files = await listRequest.ExecuteAsync();
                var folder = files.Files.FirstOrDefault();

                if (folder == null)
                {
                    var newFolder = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = name,
                        MimeType = "application/vnd.google-apps.folder",
                        Parents = new List<string> { parentId }
                    };
                    var createRequest = service.Files.Create(newFolder);
                    createRequest.Fields = "id";
                    var createdFolder = await createRequest.ExecuteAsync();
                    parentId = createdFolder.Id;
                }
                else
                {
                    parentId = folder.Id;
                }
            }
            return parentId;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderPath, string mimeType)
        {
            try
            {
                var service = await GetDriveServiceAsync();
                var parentFolderId = await GetOrCreateFolderIdAsync(service, folderPath);

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    Parents = new List<string> { parentFolderId }
                };

                var request = service.Files.Create(fileMetadata, fileStream, mimeType);
                request.Fields = "id";
                await request.UploadAsync();

                return request.ResponseBody?.Id!;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred: {ex.Message}");
                return "Error:Failed";
            }
        }

        public async Task<string> UploadDatabaseBackupAsync(string dbPath)
        {
            if (!System.IO.File.Exists(dbPath))
            {
                Console.WriteLine("Database file not found.");
                return "Error:Db not found";
            }

            using var stream = new FileStream(dbPath, FileMode.Open, FileAccess.Read);
            string fileName = $"backup_{DateTime.UtcNow:yyyyMMddHHmmss}.db";
            return await UploadFileAsync(stream, fileName, "AppBackups/SQLite", "application/octet-stream");
        }

        public async Task<string> UploadJsonAsync(string jsonContent, string fileName, string folderPath)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            return await UploadFileAsync(stream, fileName, folderPath, "application/json");
        }
        // Download Files and Folder
        /// <summary>
        /// Gets the ID of a folder path. Returns null if the path doesn't exist.
        /// </summary>
        private static async Task<string?> GetFolderIdAsync(DriveService service, string folderPath)
        {
            string parentId = "root";
            var folderNames = folderPath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var name in folderNames)
            {
                var listRequest = service.Files.List();
                listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and trashed=false and name='{name}' and '{parentId}' in parents";
                listRequest.Fields = "files(id)";
                var files = await listRequest.ExecuteAsync();
                var folder = files.Files.FirstOrDefault();

                if (folder == null)
                {
                    return null; // Folder not found
                }
                parentId = folder.Id;
            }
            return parentId;
        }

        // ------------- NEW METHODS -------------

        /// <summary>
        /// Lists all sub-folders within a specific folder path on Google Drive.
        /// </summary>
        public async Task<IList<File>> ListFoldersAsync(string folderPath)
        {
            var service = await GetDriveServiceAsync();
            var folderId = await GoogleDriveService.GetFolderIdAsync(service, folderPath);

            if (string.IsNullOrEmpty(folderId)) return new List<File>();

            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType='application/vnd.google-apps.folder' and trashed=false";
            request.Fields = "files(id, name, modifiedTime)";
            var result = await request.ExecuteAsync();
            return result.Files;
        }

        /// <summary>
        /// Lists all files (not folders) within a specific folder path on Google Drive.
        /// </summary>
        public async Task<IList<File>> ListFilesAsync(string folderPath)
        {
            var service = await GetDriveServiceAsync();
            var folderId = await GoogleDriveService.GetFolderIdAsync(service, folderPath);

            if (string.IsNullOrEmpty(folderId)) return new List<File>();

            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType!='application/vnd.google-apps.folder' and trashed=false";
            request.Fields = "files(id, name, size, modifiedTime, md5Checksum)";
            var result = await request.ExecuteAsync();
            return result.Files;
        }

        /// <summary>
        /// Lists all files in a folder that have been modified after the provided date.
        /// </summary>
        public async Task<IList<File>> ListFilesNewerThanAsync(string folderPath, DateTime lastSyncDate)
        {
            var service = await GetDriveServiceAsync();
            var folderId = await GoogleDriveService.GetFolderIdAsync(service, folderPath);

            if (string.IsNullOrEmpty(folderId)) return new List<File>();

            // Format date to RFC 3339 format required by Drive API
            string formattedDate = lastSyncDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");

            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType!='application/vnd.google-apps.folder' and trashed=false and modifiedTime > '{formattedDate}'";
            request.Fields = "files(id, name, size, modifiedTime, md5Checksum)";

            var result = await request.ExecuteAsync();
            return result.Files;
        }
        public async Task<IList<File>> ListFilesNewerThanAsync(string folderId,string folderPath,  DateTime lastSyncDate)
        {
            var service = await GetDriveServiceAsync();
            if(string.IsNullOrEmpty(folderId))
                 folderId = await GoogleDriveService.GetFolderIdAsync(service, folderPath)??"";

            if (string.IsNullOrEmpty(folderId)) return new List<File>();

            // Format date to RFC 3339 format required by Drive API
            string formattedDate = lastSyncDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");

            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType!='application/vnd.google-apps.folder' and trashed=false and modifiedTime > '{formattedDate}'";
            request.Fields = "files(id, name, size, modifiedTime, md5Checksum)";

            var result = await request.ExecuteAsync();
            return result.Files;
        }

        /// <summary>
        /// Downloads a file from a specified path in Google Drive to a local folder.
        /// </summary>
        /// <returns>The local path of the downloaded file, or null on failure.</returns>
        public async Task<string> DownloadFileAsync(string remoteFilePath, string localFolderPath)
        {
            try
            {
                var service = await GetDriveServiceAsync();

                // Separate path and filename
                string fileName = Path.GetFileName(remoteFilePath);
                string remoteFolderPath = Path.GetDirectoryName(remoteFilePath)?.Replace('\\', '/')!;

                var folderId = await GoogleDriveService.GetFolderIdAsync(service, remoteFolderPath);
                if (string.IsNullOrEmpty(folderId))
                {
                    Console.WriteLine("Remote folder not found.");
                    return "Error:Folder not found";
                }

                // Find the file by name in the parent folder
                var listRequest = service.Files.List();
                listRequest.Q = $"name='{fileName}' and '{folderId}' in parents and trashed=false";
                listRequest.Fields = "files(id)";
                var files = await listRequest.ExecuteAsync();
                var file = files.Files.FirstOrDefault();

                if (file == null)
                {
                    Console.WriteLine("Remote file not found.");
                    return "Error:File not found";
                }

                var request = service.Files.Get(file.Id);
                string localFilePath = Path.Combine(localFolderPath, fileName);

                // Ensure the local directory exists
                Directory.CreateDirectory(localFolderPath);

                using (var stream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                {
                    await request.DownloadAsync(stream);
                }

                return localFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during download: {ex.Message}");
                return "Error:Failed";
            }
        }
    }
}
