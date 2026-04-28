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
 * BaseFlyoutMenu.cs
 * 
 * Base class for all flyout menus in the application. Inherit from this class to create a new menu, and use the provided helper methods to add pages to the menu.
 * Flyout menus are a common UI pattern in mobile applications, allowing users to navigate between different sections of the app. By creating a base class for flyout menus, we can ensure consistency across the application and reduce boilerplate code when adding new pages to the menus.
 * 
 * Mobile and desktop platforms often have different UI requirements, so the AddPlatformSpecificPageTab method allows developers to specify different page types for mobile and desktop platforms. This ensures that the appropriate UI is displayed based on the device being used, providing a better user experience across all platforms.
 * Menu classes that inherit from BaseFlyoutMenu can easily add pages to the menu by calling the AddPageTab or AddPlatformSpecificPageTab methods, making it simple to create new menus and maintain a consistent navigation structure throughout the application.
 * 
 * 
 * This file defines the BaseFlyoutMenu class, which serves as a base class for all flyout menus in the application. It provides helper methods to add pages to the menu, including a new method for adding platform-specific pages.
 * The AddPlatformSpecificPageTab method allows developers to specify different page types for mobile and desktop platforms, ensuring that the appropriate UI is displayed based on the device being used.
 * To create a new menu, simply inherit from BaseFlyoutMenu and call the AddPageTab or AddPlatformSpecificPageTab methods for each page you want to include in the menu.
 */

namespace Garmetix.Base.Shells
{
    /// <summary>
    /// Base class for all flyout menus in the application. Inherit from this class to create a new menu, and use the provided helper methods to add pages to the menu.
    /// For example, you can create an "AccountingMenu" that inherits from BaseFlyoutMenu and calls AddPageTab for each page you want to add to the Accounting section of the app.
    /// This class provides a consistent way to create flyout menus across the application and reduces boilerplate code when adding new pages to the menus.
    /// </summary>
    public abstract class BaseFlyoutMenu : FlyoutItem
    {
        protected BaseFlyoutMenu(string menuTitle, string? menuIcon = null)
        {
            Title = menuTitle;
            if (menuIcon != null) Icon = menuIcon;
        }

        // 1. Your existing standard method
        protected void AddPageTab(string tabTitle, string iconFile, string routeName, Type pageType, bool isVisible = true)
        {
            var tab = new Tab { Title = tabTitle, Icon = iconFile, IsVisible = isVisible };
            tab.Items.Add(new ShellContent { Title = tabTitle, Route = routeName, ContentTemplate = new DataTemplate(pageType) });
            this.Items.Add(tab);
        }

        // 2. ADD THIS NEW METHOD for Platform-Specific Pages
        protected void AddPlatformSpecificPageTab(string tabTitle, string iconFile, string routeName, Type mobilePageType, Type desktopPageType, bool isVisible = true)
        {
            // Determine the correct page type based on the device
            Type targetPageType = (DeviceInfo.Idiom == DeviceIdiom.Desktop) ? desktopPageType : mobilePageType;

            var tab = new Tab { Title = tabTitle, Icon = iconFile, IsVisible = isVisible };
            tab.Items.Add(new ShellContent { Title = tabTitle, Route = routeName, ContentTemplate = new DataTemplate(targetPageType) });
            this.Items.Add(tab);
        }
    }
}