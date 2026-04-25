#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

using Fonts;
using Garmetix.Authentication;
using Garmetix.Authentication.Pages;
using Garmetix.CoreBase;
using Garmetix.CoreBase.Accounting;
using Garmetix.CoreBase.Dashboard;
using Garmetix.CoreBase.HRM;
using Garmetix.CoreBase.Stores;
using Garmetix.CoreServices;
using Garmetix.DataServices;
using Garmetix.Onboarding;
using Garmetix.Onboarding.Pages;
using Garmetix.PDFServices;
using Garmetix.Reports;
using Microsoft.Maui.LifecycleEvents;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using System.Globalization;

namespace Garmetix
{


    public static class GarmetixHelpers
    {
        public static async Task InitApp()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(GarmetixModulues.SyncKey);
            CultureInfo.CurrentUICulture = new CultureInfo("en-IN");
            GarmetixModulues.RegisterPaeRoutes();
        }
        


        public static async Task<Window> CreateMainWindow(IActivationState? activationState, Shell appShell)
        {

            // Check if user has previously logged in and set a PIN
            string? hasPin = await SecureStorage.Default.GetAsync("HasPin");

            if (hasPin == "true")
            {
                // Bypass full login, go straight to Quick PIN Unlock
                await Shell.Current.GoToAsync(PinUnlockPageUrl);
               // return new Window(new PinUnlockedPage(appShell));
            }

            // Check if onboarding is complete
            var onboardingComplete = Preferences.Get("IsOnboardingComplete", false);

            if (!onboardingComplete)
            {
                // Use a NavigationPage to allow navigation between onboarding/seed pages
                return new Window(new NavigationPage(new OnboardingChoicePage(new Login(appShell))));
            }
            else
            {
                // if android and ios then 

                // var session = SessionHelper.TryLoadStoreSession().Result;

                //if (!(session == null || !session.IsAutoLoginEnabled))
                //{
                //    return new Window(new AppShell());
                // }
                // else
                {
                    return new Window(new Login(appShell));
                }
            }

        }


    }
    public static class GarmetixModulues
    {

        public const string SyncKey = "sync";
        public static void RegisterPaeRoutes()
        {
            // Register any additional routes here if needed
            GarmetixCoreBaseModule.EnableCoreModulesRoutes();
            OnboardingModule.RegisterRouteOnboarding();
            AuthenticationModule.RegisterAuthenticationRoute();
        }

        public static MauiAppBuilder ConfigureGarmetixModules(this MauiAppBuilder builder)
        {

            // Enabling Syncfusion Libraries
            builder
                .ConfigureSyncfusionToolkit()
                .ConfigureSyncfusionCore()
            //Enabling Full Screen Mode for Windows
            .ConfigureLifecycleEventsFullScreen()
            // Registering Garmetix Modules
             .RegiserService().
             // Registering Garmetix Modules
             RegisterModules()
             // Enabling Fonts
             .EnableFonts();
            return builder;
        }

        public static MauiAppBuilder RegisterModules(this MauiAppBuilder builder)
        {
            builder.EnableAccounting().EnableBanking().EnableHRM().UseAuthentication();
            builder.EnableDashboard().EnableOnBoarding().EnableStore().UseReporting();

            builder.EnableSentryModule();
            //Remove this line if you don't want to enable AI billing in your app, or if you want to enable it separately in specific modules.
            //builder.EnableAIBilling().UseGarmetixSettings();
            return builder;
        }
        public static MauiAppBuilder UseGarmetixDefaultModules(this MauiAppBuilder builder)
        {
            builder
                //.UseGarmetixCoreServices()
                .EnablePdfServices().UseCoreModule().UseDataServices();

            return builder;
        }

        public static MauiAppBuilder RegiserService(this MauiAppBuilder builder)
        {
            builder.UseGarmetixCoreServices();//.UseGarmetixDatabases();
            builder.EnablePdfServices().UseCoreModule().UseDataServices();

            return builder;
        }
        /// <summary>
        /// Enables Sentry error monitoring and performance tracing for the application by configuring the Sentry module
        /// on the specified Maui app builder.
        /// </summary>
        /// <remarks>This method adds Sentry to the application's dependency injection and configures
        /// recommended options for error tracking and diagnostics. Additional Sentry options can be set by modifying
        /// the configuration within the method. Call this method during application startup to enable Sentry
        /// features.</remarks>
        /// <param name="builder">The Maui app builder to configure with Sentry integration.</param>
        /// <returns>The same Maui app builder instance, configured to use Sentry for error and performance monitoring.</returns>
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
        /// <summary>
        /// Configures the application to include a predefined set of fonts for use in the app's user interface.
        /// </summary>
        /// <remarks>This method registers several commonly used fonts, making them available for use in
        /// XAML and C# throughout the application. Call this method during app startup to ensure the fonts are
        /// available before any UI is rendered.</remarks>
        /// <param name="builder">The builder used to configure and construct the Maui application.</param>
        /// <returns>The same <see cref="MauiAppBuilder"/> instance, enabling method chaining.</returns>
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

        /// <summary>
        /// Configures the application to launch windows in full screen mode with the title bar and border visible on
        /// supported platforms.
        /// </summary>
        /// <remarks>This extension method customizes window lifecycle events to maximize the window and
        /// display the title bar and border on Windows platforms. On other platforms, this method has no
        /// effect.</remarks>
        /// <param name="builder">The builder used to configure the Maui application and its lifecycle events.</param>
        /// <returns>The same builder instance, enabling method chaining.</returns>
        public static MauiAppBuilder ConfigureLifecycleEventsFullScreen(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
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
         });

            return builder;
        }


    }
}
