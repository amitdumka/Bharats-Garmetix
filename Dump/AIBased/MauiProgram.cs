using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Syncfusion.Maui.Core.Hosting;
using AadwikaBilling.Services;
using AadwikaBilling.ViewModels;
using AadwikaBilling.Views;

namespace AadwikaBilling
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore() // Crucial for UI
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Platform-Specific Printing
#if ANDROID
            builder.Services.AddSingleton<IPrintService, AadwikaBilling.Platforms.Android.PrintService>();
#elif WINDOWS
            builder.Services.AddSingleton<IPrintService, AadwikaBilling.Platforms.Windows.PrintService>();
#endif

            // Register Views and ViewModels
            builder.Services.AddTransient<InvoiceEntryViewModel>();
            builder.Services.AddTransient<InvoiceEntryPage>();

            return builder.Build();
        }
    }
}