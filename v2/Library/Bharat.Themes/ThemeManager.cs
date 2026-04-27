using Microsoft.Maui.Controls;

namespace Bharat.Themes
{
    public enum AppThemeMode
    {
        Light,
        Dark,
        SystemDefault
    }

    public static class ThemeManager
    {
        public static void SetTheme(AppThemeMode themeMode)
        {
            if (Application.Current == null) return;

            // 1. Tell the MAUI OS what theme we are in (helps with native controls like status bars)
            Application.Current.UserAppTheme = themeMode switch
            {
                AppThemeMode.Light => AppTheme.Light,
                AppThemeMode.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            // 2. Swap out the Resource Dictionaries dynamically
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            if (mergedDictionaries != null)
            {
                // Remove existing theme dictionaries
                var existingLightTheme = mergedDictionaries.FirstOrDefault(d => d.GetType() == typeof(Light));
                var existingDarkTheme = mergedDictionaries.FirstOrDefault(d => d.GetType() == typeof(Dark));

                if (existingLightTheme != null) mergedDictionaries.Remove(existingLightTheme);
                if (existingDarkTheme != null) mergedDictionaries.Remove(existingDarkTheme);

                // Determine active theme based on System preference if set to SystemDefault
                bool useDarkTheme = themeMode == AppThemeMode.Dark ||
                                   (themeMode == AppThemeMode.SystemDefault && Application.Current.RequestedTheme == AppTheme.Dark);

                // Inject the correct dictionary
                if (useDarkTheme)
                {
                    mergedDictionaries.Add(new Dark());
                }
                else
                {
                    mergedDictionaries.Add(new Light());
                }
            }
        }
    }
}