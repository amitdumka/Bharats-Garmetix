namespace Garmetix.DataServices
{
    // All the code in this file is included in all platforms.
    // All the code in this file is included in all platforms.
    public static class DataServicesModule
    {

        extension(MauiAppBuilder builder)
        {
            public MauiAppBuilder UseDataServices()
            {
                //Databse Backup Services
                builder.Services.AddSingleton<BackupService>();
                builder.Services.AddSingleton<JsonDatabaseBackupService>();
                builder.Services.AddSingleton<IDatabaseBackupService, DatabaseBackupService>();

                return builder;
            }
        }
    }
}
