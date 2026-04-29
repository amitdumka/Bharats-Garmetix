using Bharat.ToolKits.Helpers;
using Garmetix.Databases;
using Garmetix.Databases.Services;

namespace Garmetix.Data.Databases
{
    // All the code in this file is included in all platforms.
    public static class GarmetixDatabasesModule
    {
        public static MauiAppBuilder UseGarmetixDatabases(this MauiAppBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDatabaseContext>();
            //LocalDataBase Context
            Constants.CompanyDatabaseFileName = StorageOps.GetPref("CompanyDatabaseFileName", "dummy.db");
            builder.Services.AddDbContext<DatabaseContext>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddTransient<LocalDatabaseService>();
            return builder;
        }
    }
}
