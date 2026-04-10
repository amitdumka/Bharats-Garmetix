namespace Garmetix.Settings
{
    // All the code in this file is included in all platforms.
    public static class GarmetixSettingsModule
    {
        // Setting Module Version 2.0.0
        // Enabling this module will allow you to use the Garmetix Settings System in your project.
        public static MauiAppBuilder UseGarmetixSettings(this MauiAppBuilder builder)
        {
            // Register the Garmetix Settings Service
            // builder.Services.AddSingleton<IGarmetixSettingsService, GarmetixSettingsService>();
            return builder;
        }
        //public static IRouteBuilder EnableSettingRoute(this IRouteBuilder builder)
        //{
        //    // Map the Garmetix Settings Route
        //    builder.MapRoute("garmetixsettings", "garmetixsettings", new { controller = "GarmetixSettings", action = "Index" });
        //    return builder;
        //}
    }
}
