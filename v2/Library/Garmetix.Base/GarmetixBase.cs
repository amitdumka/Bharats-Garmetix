namespace Garmetix.Base
{

    //this will be  core and core based UI libraries, and will be used by all the other libraries
    // All the code in this file is included in all platforms.
    public  static class GarmetixBase
    {
        
        public static MauiAppBuilder UseGarmetixBase(this MauiAppBuilder builder)
        {
            // Register services, handlers, etc. here
            // For example:
            // builder.Services.AddSingleton<IMyService, MyService>();
            return builder;
        }
    }
}
