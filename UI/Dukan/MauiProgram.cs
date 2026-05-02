using CommunityToolkit.Maui;
using Garmetix.Data.Databases;
using Garmetix.Dependencies;
using Garmetix.SRP;
using Microsoft.Extensions.Logging;
namespace Dukan
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() //Placeholder to ensure the toolkit is registered before any module, as some modules might depend on it.
                .UseGarmetixDependencies() // Registering Garmetix dependencies and global configurations                      
                .UseGarmetixSRP()  // Registering Garmetix SRP modules and their routes
                .UseGarmetixDatabases();   // Registering Garmetix Databases
                //.ConfigureFonts(fonts =>
                //{
                //    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                //    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                //});

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
