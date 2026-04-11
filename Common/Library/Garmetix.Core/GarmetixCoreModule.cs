namespace Garmetix.Core
{
    public static class GarmetixCoreModule
    {
        public static MauiAppBuilder UseGarmetixCore(this MauiAppBuilder builder)
        {
            // Register any services or dependencies here if needed
            // e.g., builder.Services.AddSingleton<IMyService, MyService>();

            // Register the Core/Support Module
            builder.Services.AddTransient<Garmetix.Core.ViewModels.AboutUsViewModel>();
            builder.Services.AddTransient<Garmetix.Core.Views.AboutUsPage>();

            builder.Services.AddTransient<Garmetix.Core.ViewModels.ContactUsViewModel>();
            builder.Services.AddTransient<Garmetix.Core.Views.ContactUsPage>();

            return builder;
        }
    }
}
