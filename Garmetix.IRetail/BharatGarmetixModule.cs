using Fonts;
using Microsoft.Maui.LifecycleEvents;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using Garmetix.CoreBase.Accounting;
using Garmetix.CoreBase.HRM;
using Garmetix.CoreBase.Dashboard;
using Garmetix.Onboarding;
using Garmetix.CoreBase.Stores;
using Garmetix.PDFServices;
using Garmetix.CoreBase;
using Garmetix.DataServices;
using Garmetix.CoreServices;
using Garmetix.Authentication;
using Garmetix.Reports;
using Garmetix.Billing;
using Garmetix.Settings;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace Garmetix.IRetail
{
    internal static class BharatGarmetixModules
    {
        public static MauiAppBuilder RegiserService(this MauiAppBuilder builder)
        {
            builder.UseGarmetixCoreServices();//.UseGarmetixDatabases();
            builder.EnablePdfServices().UseCoreModule().UseDataServices();
            
            return builder;
        }

        public static MauiAppBuilder RegisterModules(this MauiAppBuilder builder)
        {
            builder.EnableAccounting().EnableBanking().EnableHRM().UseAuthentication();
            builder.EnableDashboard().EnableOnBoarding().EnableStore().UseReporting();
            builder.EnableSentryModule();
            builder.EnableAIBilling().UseGarmetixSettings();
            return builder;
        }

        public static MauiAppBuilder EnableSentryModule(this MauiAppBuilder builder)
        {
            // Add this section anywhere on the builder:
            builder.UseSentry(options =>
            {
                // The DSN is the only required setting.
                options.Dsn = "https://0104adb654cb4511dc40a771542f441a@o4509464503386112.ingest.us.sentry.io/4509464506138624";
                // Use debug mode if you want to see what the SDK is doing.
                // Debug messages are written to stdout with Console.Writeline,
                // and are viewable in your IDE's debug console or with 'adb logcat', etc.
                // This option is not recommended when deploying your application.
                options.Debug = true;
                // Adds request URL and headers, IP and name for users, etc.
                options.SendDefaultPii = true;
                // This option is recommended. It enables Sentry's "Release Health" feature.
                options.AutoSessionTracking = true;
                // Enabling this option is recommended for client applications only. It ensures all threads use the same global scope.
                options.IsGlobalModeEnabled = false;
                // Example sample rate for your transactions: captures 10% of transactions
                options.TracesSampleRate = 0.1;
                // Other Sentry options can be set here.
                // By default it's already the most verbose level: Debug
                // You can use this make this less noisy by changing it to
                // a less verbose level such as `Information` or `Warning`.
                options.DiagnosticLevel = SentryLevel.Debug;
            });
            return builder;
        }
        public static void RegisterPaeRoutes()
        {
            // Register any additional routes here if needed
            GarmetixCoreBaseModule.EnableCoreModulesRoutes();
            OnboardingModule.RegisterRouteOnboarding();
            AuthenticationModule.RegisterAuthenticationRoute();
        }

        public static MauiAppBuilder EnableBharatGarmetixModules(this MauiAppBuilder builder)
        {
            builder.ConfigureSyncfusionToolkit()
                   .ConfigureSyncfusionCore()

                .ConfigureLifecycleEvents(events =>
                {
#if WINDOWS
                    events.AddWindows(w =>
                    {
                        events.AddWindows(windowsLifecycleBuilder =>
                            {
                                windowsLifecycleBuilder.OnWindowCreated(window =>
                                {
                                    window.ExtendsContentIntoTitleBar = false;
                                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                                    var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                                    switch (appWindow.Presenter)
                                    {
                                        case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                                            overlappedPresenter.SetBorderAndTitleBar(true, true);
                                            overlappedPresenter.Maximize();
                                            break;
                                    }
                                });
                            });
                    });
#endif
                })
                .RegiserService().RegisterModules().EnableFonts();
            return builder;
        }

        public static MauiAppBuilder EnableFonts(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("MauiMaterialAssets.ttf", "MaterialAssets");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("UIFontIcons.ttf", "FontIcons");
                fonts.AddFont("Dashboard.ttf", "DashboardFontIcons");
                fonts.AddFont("Roboto-Medium.ttf", "Roboto-Medium");
                fonts.AddFont("Roboto-Regular.ttf", "Roboto-Regular");

                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });
            return builder;
        }
    }
}