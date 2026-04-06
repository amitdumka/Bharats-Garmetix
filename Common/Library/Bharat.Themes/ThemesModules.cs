namespace Bharat.Themes
{
    public enum Themes
    {
        Light,
        Dark,
        SystemDefault
    }
    public class ThemesModules
    {
        public Themes SelectedThemes { get; set; } = Themes.Light;
        public ThemesModules()
        {
            // Default constructor
        }
        public ThemesModules(Themes selectedThemes)
        {
            SelectedThemes = selectedThemes;
        }
    }
}
