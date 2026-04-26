//using Plugin.LocalNotification;

using Garmetix.Databases;
using Microsoft.Extensions.Logging;


namespace Garmetix.IRetail
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                    .ConfigureMauiHandlers(handlers =>
                    {
#if IOS || MACCATALYST
                     handlers.AddHandler<CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                    })
                .UseGarmetixDatabases()
                ;
#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            return builder.Build();
        }
    }
}
