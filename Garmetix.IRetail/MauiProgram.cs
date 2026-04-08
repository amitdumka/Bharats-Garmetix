using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
//using Plugin.LocalNotification;

using Garmetix.Databases;


namespace Garmetix.IRetail
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>().EnableBharatGarmetixModules()
                    .ConfigureMauiHandlers(handlers =>
                    {
#if IOS || MACCATALYST
                     handlers.AddHandler<CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                    })
                .UseGarmetixDatabases()
                .UseMauiCommunityToolkit(options =>
                {
                    options.SetShouldEnableSnackbarOnWindows(true);
                });
#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            return builder.Build();
        }
    }
}
