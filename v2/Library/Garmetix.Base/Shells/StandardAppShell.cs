/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/*
 * StandardAppShell.cs
 *
 * This file defines the StandardAppShell class, which serves as a base class for application shells in the Garmetix ecosystem. It provides a consistent flyout menu structure and styling, along with support for dynamic theming and common menu items like Logout and Quit. Application-specific shells can inherit from StandardAppShell and override the abstract methods to define their unique menu structure and navigation routes, while still benefiting from the shared styling and common functionality provided by the base class.
 * StandardAppShell is designed to be used in any Garmetix application that wants to follow the standard flyout menu design and behavior, ensuring a cohesive user experience across the ecosystem. By centralizing common shell functionality in StandardAppShell, we can reduce code duplication and make it easier to maintain a consistent look and feel across all Garmetix applications.
 * Since this is a base class, it does not contain any application-specific menu items or routes. Instead, it defines abstract methods that derived classes must implement to build their unique menu structure and register their navigation routes. This allows each application to have its own distinct menu while still adhering to the overall design and functionality standards set by StandardAppShell.
 */

namespace Garmetix.Base.Shells
{
    /// <summary>
    /// StandardAppShell is a base class for application shells that want to implement a consistent flyout menu structure and styling across all Garmetix applications. It provides built-in support for dynamic theming, custom headers/footers, and standard menu items like Logout and Quit. Application-specific shells can inherit from StandardAppShell and override the abstract methods to define their unique menu structure and navigation routes, while still benefiting from the shared styling and common functionality provided by the base class.
    /// should be used in any app that wants to follow the standard Garmetix flyout menu design and behavior, ensuring a consistent user experience across the ecosystem. By centralizing common shell functionality in StandardAppShell, we can reduce code duplication and make it easier to maintain a cohesive look and feel across all Garmetix applications.
    /// </summary>
    public abstract class StandardAppShell : Shell
    {
        protected readonly StandardShellConfig _config;

        protected StandardAppShell(StandardShellConfig config)
        {
            _config = config;

            ApplyStandardStyling();
            ApplyCustomHeadersAndFooters();

            BuildAppSpecificMenu();
            BuildStandardFooterMenus();
            RegisterAppRoutes();
        }

        private void ApplyStandardStyling()
        {
            FlyoutBehavior = FlyoutBehavior.Flyout;

            // Dynamic Light/Dark Mode Binding
            this.SetAppThemeColor(Shell.FlyoutBackgroundColorProperty, _config.BackgroundColorLight, _config.BackgroundColorDark);

            // Background Image
            if (!string.IsNullOrEmpty(_config.FlyoutBackgroundImage))
            {
                FlyoutBackgroundImage = _config.FlyoutBackgroundImage;
                FlyoutBackgroundImageAspect = _config.FlyoutBackgroundImageAspect;
            }

            FlyoutHeaderBehavior = _config.HeaderBehavior;
        }

        private void ApplyCustomHeadersAndFooters()
        {
            if (_config.CustomHeaderView != null)
                FlyoutHeader = _config.CustomHeaderView;

            if (_config.CustomFooterView != null)
                FlyoutFooter = _config.CustomFooterView;
        }

        private void BuildStandardFooterMenus()
        {
            if (_config.IncludeStandardLogoutMenu)
            {
                var logoutItem = new MenuItem { Text = "Logout", StyleClass = ["MenuItemLayoutStyle"] };
                logoutItem.Clicked += async (s, e) => await HandleLogout();
                Items.Add(logoutItem);

                var quitItem = new MenuItem { Text = "Quit", StyleClass = ["MenuItemLayoutStyle"] };
                quitItem.Clicked += (s, e) => HandleQuit();
                Items.Add(quitItem);
            }
        }

        // --- ABSTRACT HOOKS ---
        protected abstract void BuildAppSpecificMenu();
        protected abstract void RegisterAppRoutes();

        // Virtual methods allow the specific app to define *how* logout and quit work
        protected virtual Task HandleLogout() => Task.CompletedTask;
        protected virtual void HandleQuit() { }
    }

     
}
