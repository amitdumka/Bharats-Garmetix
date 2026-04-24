 

namespace Garmetix.SRP
{
    // All the code in this file is included in all platforms.
    public static class GarmetixSRPUIModule
    {
        public static MauiAppBuilder ConfigureGarmetixSRPUI(this MauiAppBuilder builder)
        {
            // Register services, handlers, etc. here
            // For example:
            // builder.Services.AddSingleton<IMyService, MyServiceImplementation>();
            return builder;
        }


        public static bool EnableSRPUIRoutes()
        {
            // Enable routing for SRP UI components
            return true;
        }


    }



}
