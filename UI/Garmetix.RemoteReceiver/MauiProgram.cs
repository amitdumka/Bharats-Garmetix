using Garmetix.RemoteReceiver.Services;
using Garmetix.RemoteReceiver.ViewModels;
using Garmetix.RemoteReceiver.Views;
using Microsoft.Extensions.Logging;

namespace Garmetix.RemoteReceiver
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            // 1. Register the UI and ViewModels
            builder.Services.AddTransient<RemoteReceiverPage>();
            builder.Services.AddTransient<RemoteReceiverViewModel>();
            // 2. Register the Hardware Printer Interface based on the platform
#if ANDROID
            builder.Services.AddSingleton<IPlatformPrinter, Platforms.Android.AndroidBluetoothPrinter>();
#elif WINDOWS
        builder.Services.AddSingleton<IPlatformPrinter, Platforms.Windows.WindowsUsbPrinter>();
#endif
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
