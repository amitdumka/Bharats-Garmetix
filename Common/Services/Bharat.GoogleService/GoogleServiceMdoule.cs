using Bharat.GoogleDrive.Services; 

namespace Bharat.GoogleService
{
    // All the code in this file is included in all platforms.
    public static class GoogleServiceMdoule
    {
        public static MauiAppBuilder UseGoogleService(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IGoogleDriveService, GoogleDriveService>();
            return builder;
        }
    }
}
