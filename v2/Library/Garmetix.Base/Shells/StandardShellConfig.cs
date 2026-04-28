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
 * This file is part of Garmetix, a collection of .NET MAUI controls and utilities designed to streamline app development and enhance user experience. Garmetix provides a wide range of customizable components, including the StandardAppShell, which serves as a base class for creating consistent and visually appealing flyout menus across all Garmetix applications. The StandardShellConfig class encapsulates all the configurable properties for the StandardAppShell, allowing developers to easily customize the appearance and behavior of their flyout menus while maintaining a cohesive design language throughout the Garmetix ecosystem.
 * 
 * For more information about Garmetix and its components, please visit our GitHub repository:
 * https://github.com/garmetix
 * 
 * StandardShellConfig.cs is licensed under the MIT License. See the LICENSE file in the project root for more information.
 *
 *Should you have any questions or need further assistance, please feel free to contact us at   
 *Shelll 
 */
namespace Garmetix.Base.Shells
{
    /// <summary>
    /// StandardShellConfig is a configuration class that encapsulates all the customizable properties for the StandardAppShell. It allows developers to easily configure the appearance and behavior of the flyout menu, including theme colors, background images, header/footer views, and standard menu item inclusion. By using StandardShellConfig, developers can create a consistent look and feel across all Garmetix applications while still allowing for application-specific customization through the StandardAppShell base class.
    /// StandardShellConfig provides a centralized way to manage all the styling and configuration options for the StandardAppShell, making it easier to maintain and update the flyout menu design across the entire Garmetix ecosystem. Developers can create different instances of StandardShellConfig for different applications, allowing each app to have its own unique styling while still adhering to the overall design principles set by the StandardAppShell.
    /// </summary>
    public class StandardShellConfig
    {
        public string AppName { get; set; } = "Garmetix";

        // Theme Colors
        public Color BackgroundColorLight { get; set; } = Colors.White;
        public Color BackgroundColorDark { get; set; } = Color.FromArgb("#121212");

        // Flyout Styling
        public string FlyoutBackgroundImage { get; set; } = string.Empty;
        public Aspect FlyoutBackgroundImageAspect { get; set; } = Aspect.Fill;
        public FlyoutHeaderBehavior HeaderBehavior { get; set; } = FlyoutHeaderBehavior.CollapseOnScroll;

        // Custom Views for Header and Footer
        public View? CustomHeaderView { get; set; }
        public View? CustomFooterView { get; set; }

        public bool IncludeStandardLogoutMenu { get; set; } = true;
    }

  
}
