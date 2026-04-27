using SQLite;

namespace Garmetix.Databases
{
    public static class Constants
    {
        public const string AppName = "BharatGarmetix";
        public const string DatabaseFilename = "_bharatGarmetixdb.db3";
        public static string CompanyDatabaseFileName = "dummy_bharatGarmetixdb.db3";

        public const SQLiteOpenFlags Flags = // Updated to use the resolved 'SQLite' namespace.
                                             // open the database in read/write mode
            SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.FullMutex |
            // enable multi-threaded database access
            SQLiteOpenFlags.SharedCache;

        public static string DatabaseName = "AppSQLite.db3";

        public static string BharatGarmetixDatabasePath =>
            $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseName)}";

        public static string DatabasePath =>
            Path.Combine(FileSystem.Current.AppDataDirectory, DatabaseFilename);

        public static string CompanyDatabasePath =>
            Path.Combine(FileSystem.Current.AppDataDirectory, CompanyDatabaseFileName);

        public static string CompanyDatabaseExternalPath =>
            Path.Combine(SetExternalBasePath(), CompanyDatabaseFileName);

        //TODO: Enable External Storage move to Setting section
        public static bool EnableExternalStorage = false;

        /// <summary>
        /// Set External Base Path for Database Storage in different devics
        /// </summary>
        /// <returns></returns>
        public static string SetExternalBasePath()
        {
            string basePath = "";

#if ANDROID
            // On Android: Use the external storage directory.
            // IMPORTANT: Make sure your AndroidManifest.xml requests the necessary permissions,
            // and handle runtime permissions if targeting Android 6.0+.
            var extRoot = Android.App.Application.Context.GetExternalFilesDir(null)?.ToString();
            if (extRoot != null)
            {
                basePath = Path.Combine(extRoot, "BharatRetails", "Databases");
            }
            else
            {
                Console.WriteLine("External storage directory is null.");
                basePath = Path.Combine(FileSystem.AppDataDirectory, "BharatRetails", "Databases");
            }
#elif IOS
            // On iOS, there's no true "external" storage, however, files located in the Documents folder can
            // be shared via iTunes file sharing (if enabled) or the Files app.
            // To enable file sharing, set UIFileSharingEnabled to YES in your Info.plist.
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            basePath = Path.Combine(docs, "BharatRetails", "Databases");
#elif WINDOWS
            // On Windows, use an external path as specified.
            basePath = Path.Combine(@"C:\BharatRetails\Databases");
#else
            // For other platforms, try using a common external folder if available.
            basePath = Path.Combine(FileSystem.AppDataDirectory, "BharatRetails", "Databases");
#endif

            // Ensure the directory exists.
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            return basePath;
        }
    }
}