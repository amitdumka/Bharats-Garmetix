using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Converters;
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
using Garmetix.Databases;
using Garmetix.Authentication;
using Garmetix.Reports;
using Garmetix.Models.Stores;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace Garmetix
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